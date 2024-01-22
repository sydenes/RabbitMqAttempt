using FileCreateWorkerService;
using FileCreateWorkerService.Services;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var connectionString = builder.Configuration.GetConnectionString("RabbitMq");
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(connectionString), DispatchConsumersAsync = true });
builder.Services.AddSingleton<RabbitMqClientService>();

var host = builder.Build();
host.Run();
