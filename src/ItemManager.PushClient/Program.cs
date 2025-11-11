using ItemManager.Core.Models;
using ItemManager.Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<FirebasePushOptions>(
    builder.Configuration.GetSection("FirebasePushOptions"));
builder.Services.AddHttpClient<FirebasePushNotificationService>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

app.UseResponseCompression();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.Run();
