using ClosedXML.Excel;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Data;
using System.Text;
using System.Text.Json;

namespace FileCreateWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqClientService _rabbitMqClientService;
        private readonly IServiceProvider _serviceProvider; //IServiceProvider ile DI Container'a eriþim saðlanýr. Context scoped olarak tanýmlandýðý için doðrudan kullanamayýz, Singelton tanýmlanmýþ RabbitMqClientService doðrudan kullanýlabilir.
        private IModel _channel;

        public Worker(ILogger<Worker> logger, RabbitMqClientService rabbitMqClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMqClientService = rabbitMqClientService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel=_rabbitMqClientService.Connect();
            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(RabbitMqClientService.QueueName, false, consumer);

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is called when a message is received from the queue. Received uploaded excel and call the method where ExcelCreateProject's FilesController's Upload method is called to upload the excel file to the server (wwwroot/files).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);

            var excel=JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            using (var ms=new MemoryStream())
            {
                var wb = new XLWorkbook();
                var ds = new DataSet();
                ds.Tables.Add(GetTable("Products"));

                wb.Worksheets.Add(ds);
                wb.SaveAs(ms);

                MultipartFormDataContent multipartFormDataContent = new();
                multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()), "file", Guid.NewGuid().ToString() + ".xlsx");

                var baseUrl = "https://localhost:7163/api/files";
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync($"{baseUrl}?fileId={excel.UserFileId}", multipartFormDataContent);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"File (id: {excel.UserFileId}) created on the server side");
                        _channel.BasicAck(@event.DeliveryTag, false); //when message is processed successfully, we will delete it from the queue.
                    }
                }
            }
        }

        private DataTable GetTable(string tableName)
        {
            List<Product> products = new List<Product>();
            using (var scope= _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AdventureWorks2019Context>();
                products = context.Products.ToList();
            }

            DataTable table = new DataTable(tableName);
            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("ProductNumber", typeof(string));
            table.Columns.Add("Color", typeof(string));

            products.ForEach(p =>
            {
                table.Rows.Add(p.ProductId, p.Name, p.ProductNumber, p.Color);
            });
            return table;
        }
    }
}
