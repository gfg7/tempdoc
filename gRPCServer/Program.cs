using gRPCContract.Interfaces;
using gRPCContract.Models.Stored;
using gRPCContract.Utils;
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
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<ErrorHandler>();

builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = builder.Environment.IsDevelopment();

    o.EnableDetailedErrors = false;

    o.Interceptors.Add<GrpcErrorHandler>();
    o.IgnoreUnknownServices = false;

    int.TryParse(Env.Get("DEFAULT_MAX_FILE_SIZE"), out int size);
    o.MaxReceiveMessageSize = size is 0 ? null : size;
}).AddJsonTranscoding();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo()
        {
            Title = "TempDocSaver",
            Version = "v1",
            Description = "gRPC transcoding API for web clients of TempDocSaver"
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
        o.DisallowConcurrentExecution(false);
        o.WithIdentity(key);
        o.RequestRecovery(true);
    });

    t.AddTrigger(q =>
    {
        q.ForJob(nameof(DropExpiredJob));
        // q.WithCronSchedule("0 */15 * ? * *");
        q.WithCronSchedule(Env.Get("CRON_FLUSH_EXPIRED"));
    });
});

builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = null;
});

var app = builder.Build();

app.UseMiddleware<WebErrorHandler>();

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
    });
}

app.MapHealthChecks("/heath", new()
{
    AllowCachingResponses = false
});

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
    var (filename, file) =await service.GetFile(bucket, code);
    file.Position = 0;
    return Results.Stream(file, "application/octet-stream", filename);
}).WithTags("TempDocSaver");

app.MapGrpcService<TempDocSaverHandler>();

app.UseResponseCaching();

// app.UseCors(o =>
// {

// });

app.Run();
