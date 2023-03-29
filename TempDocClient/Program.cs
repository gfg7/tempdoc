using ProtoContract.Protos;
using TempDocClient.Services;
using WebContract.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<TempDocSaver.TempDocSaverClient>(o =>
{
    o.Address = new Uri(Environment.GetEnvironmentVariable("TEMPDOC_HOST"));
}).ConfigureChannel(x=> {
    x.UnsafeUseInsecureChannelCallCredentials = true;
});
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IClientBucketManagement, TempDocSaverHandler>();

// builder.Services.AddSignalRCore();

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

// app.MapHub<TempDocHub>("/");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
