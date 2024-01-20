using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitmqWatermark.BackgroundServices;
using RabbitmqWatermark.Models;
using RabbitmqWatermark.Services;

var builder = WebApplication.CreateBuilder(args);

//RabbitMq conection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("RabbitMq");
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(connectionString), DispatchConsumersAsync=true }); //Background servisinde Async metotlar kullandýðýmýz için DispatchConsumersAsync=true ile bunun kullanýmýný açtýk

builder.Services.AddSingleton<RabbitMqClientService>();

builder.Services.AddSingleton<RabbitMqPublisher>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options=>options.UseInMemoryDatabase("productDb"));

builder.Services.AddHostedService<ImageWatermarkBackgroundService>();

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
