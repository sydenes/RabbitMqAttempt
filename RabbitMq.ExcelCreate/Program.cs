using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMq.ExcelCreate;
using RabbitMq.ExcelCreate.Models;
using RabbitMq.ExcelCreate.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//RabbitMq conection string from appsettings.json
var connectionStringRabbitMq = builder.Configuration.GetConnectionString("RabbitMq");
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(connectionStringRabbitMq), DispatchConsumersAsync = true }); //Background servisinde Async metotlar kullandýðýmýz için DispatchConsumersAsync=true ile bunun kullanýmýný açtýk

builder.Services.AddSingleton<RabbitMqClientService>();
builder.Services.AddSingleton<RabbitMqPublisher>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerExcel")));
//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.SignIn.RequireConfirmedEmail = true;
//});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

//DbContext Seeder Extension
builder.Services.DbSeederExtension();


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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
