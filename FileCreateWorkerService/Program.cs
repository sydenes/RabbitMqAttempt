using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var connectionStringSqlAdventureWorks = builder.Configuration.GetConnectionString("SqlServerAdventureWorks");
builder.Services.AddDbContext<AdventureWorks2019Context>(options =>
    options.UseSqlServer(connectionStringSqlAdventureWorks));

var connectionStringRabbitMq = builder.Configuration.GetConnectionString("RabbitMq");
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(connectionStringRabbitMq), DispatchConsumersAsync = true });


builder.Services.AddSingleton<RabbitMqClientService>();

var host = builder.Build();
host.Run();
