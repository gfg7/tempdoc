using gRPCContract.Interfaces;
using gRPCContract.Protos;
using gRPCContract.Utils;
using TempDocClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<TempDocSaver.TempDocSaverClient>(o=> {
    o.Address = new Uri(Env.Get("TEMPDOC_HOST"));
});
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IClientBucketManagement, TempDocSaverHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
