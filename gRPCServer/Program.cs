using System.Net;
using gRPCServer.Intefaces.DB;
using gRPCServer.Intefaces.Repository;
using gRPCServer.Intefaces.Services;
using gRPCServer.Jobs;
using gRPCServer.Middleware;
using gRPCServer.Services.DB;
using gRPCServer.Services.ErrorHandling;
using gRPCServer.Services.Management;
using gRPCServer.Services.ProtosHandler;
using gRPCServer.Services.Repository;
using gRPCServer.Services.Utils;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Quartz;
using WebContract.Interfaces;
using WebContract.Models.Stored;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(o=> {
    o.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties;
});

builder.Logging.AddConsole();

builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOutputCache();

builder.Services.AddCors();

builder.Services.AddSingleton<ErrorHandler>();

builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = builder.Environment.IsDevelopment();

    o.Interceptors.Add<GrpcErrorHandler>();
    o.IgnoreUnknownServices = false;

    int.TryParse(Env.Get("DEFAULT_MAX_FILE_SIZE"), out int size);
    o.MaxReceiveMessageSize = size is 0 ? null : size;
}).AddJsonTranscoding();

builder.Services.AddGrpcSwagger();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                     ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo()
        {
            Title = "TempDocSaver",
            Version = "v1",
            Description = "API for web clients of TempDocSaver"
        }
    );
});

builder.Services.AddScoped<IMongoClient, MongoClient>(o => new MongoClientFactory().Client);
builder.Services.AddScoped<IDBContext, DBContext>();
builder.Services.AddScoped<IDBContext<StoredFileInfo>, DBContext>();

builder.Services.AddScoped<IFilesRepository, FilesRepository>();
builder.Services.AddScoped(typeof(IInfoRepository<>), typeof(InfoRepository<>));

builder.Services.AddScoped<IClientBucketManagement, BucketManagementService>();
builder.Services.AddScoped<IBucketRemoval, BucketManagementService>();

builder.Services.AddQuartz(t =>
{
    t.UseMicrosoftDependencyInjectionJobFactory();

    var key = nameof(DropExpiredJob);
    t.AddJob<DropExpiredJob>(o =>
    {
        o.DisallowConcurrentExecution(true);
        o.WithIdentity(key);
        o.RequestRecovery(true);
    });

    t.AddTrigger(q =>
    {
        q.ForJob(nameof(DropExpiredJob));
        q.WithCronSchedule(Env.Get("CRON_FLUSH_EXPIRED"));
    });
});

builder.Services.AddQuartzHostedService(o=> {
    o.AwaitApplicationStarted = true;
    o.StartDelay = null;
    o.WaitForJobsToComplete = true;
});

builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = null;
    o.Listen(IPAddress.Any, 81, listenOptions => {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

var app = builder.Build();

app.UseHttpLogging();

app.UseMiddleware<WebErrorHandler>();

  app.UseForwardedHeaders();
  app.UseRouting();

if (builder.Environment.IsDevelopment() || bool.Parse(Env.Get("SHOW_API")))
{
    app.UseSwagger(o=> {
        o.PreSerializeFilters.Add((swagger, request) =>
        {
            swagger.Servers.Add(new()
            {
                Url = $"{request.Scheme}://{request.Host.Value}/{request.Headers["X-Forwarded-Prefix"]}"
            });
        });
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
    });
}

app.MapHealthChecks("/health", new()
{
    AllowCachingResponses = false,
    ResultStatusCodes = null
}).RequireHost("*:80");

app.MapPost("/api/{bucket}/files", async (
    [FromRoute] string bucket,
    IFormFileCollection docs,
    IClientBucketManagement service) =>
{
    return await service.UploadFiles(bucket, docs);
}).Accepts<IFormFileCollection>("multipart/form-data").WithTags("TempDocSaver");

app.MapGet("/api/{bucket}/{code}", async (
    [FromRoute] string bucket,
    [FromRoute] string code,
    IClientBucketManagement service) =>
{
    var (filename, file) = await service.GetFile(bucket, code);
    file.Position = 0;
    return Results.Stream(file, "application/octet-stream", filename);
}).WithTags("TempDocSaver")
.CacheOutput(o =>
{
    if (int.TryParse(Env.Get("CACHE_LIFETIME"), out int cache) && cache is not 0)
    {
        o.Cache();
        o.Expire(TimeSpan.FromMinutes(cache));
    } else {
        o.NoCache();
    }
});

app.MapGrpcService<TempDocSaverHandler>().RequireHost("*:81");

app.UseOutputCache();

app.UseCors(x=> {
    x.AllowAnyHeader();
    x.AllowAnyMethod();
    x.AllowAnyOrigin();
});

app.Run();
