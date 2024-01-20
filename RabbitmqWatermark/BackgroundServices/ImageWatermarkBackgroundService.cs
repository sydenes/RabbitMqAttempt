
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitmqWatermark.Services;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitmqWatermark.BackgroundServices
{
    public class ImageWatermarkBackgroundService : BackgroundService //RabbitMq service'leri BackgroundService olmadan da çalışır ancak bu servis programın 3 zamanında tetiklenerek işlem yapmanı sağlar.(StartAsync, ExecuteAsync, StopAsync ). Örneğin program kapanırken kuyrukta mesaj varsa bunları şu dosyaya yaz gibi belirli zamanlarda belirli işlemler.
    {
        private readonly RabbitMqClientService _rabbitMqClientService;

        private readonly ILogger<ImageWatermarkBackgroundService> _logger;
        private IModel _channel;
        public ImageWatermarkBackgroundService(RabbitMqClientService rabbitMqClientService, ILogger<ImageWatermarkBackgroundService> logger)
        {
            _rabbitMqClientService = rabbitMqClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel=_rabbitMqClientService.Connect();
            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel); //Oluşturulan RabbitMq kanalını tüketecek nesne.

            _channel.BasicConsume(RabbitMqClientService.QueueName,false, consumer);

            //consumer.Received += (sender, @event) => { } eventi bu şekilde Lambda fonksiyon şeklinde de kullanabilirdik ancak çok kod bloğu uzamaması için onun yarine aşağıdaki gibi ayrı bir metotta yönettik.
            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            Task.Delay(5000).Wait(); //for watching process
            try
            {
                //Image yüklendiğinde tetiklenen event
                var imageCreatedEvent = JsonSerializer.Deserialize<productImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                //Resim üzerine yazı yazma işleri(önemli değil)
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", imageCreatedEvent?.ImageName);
                var text = "www.mysite.com";
                using var img = Image.FromFile(path);
                using var graphic = Graphics.FromImage(img);
                var font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold, GraphicsUnit.Pixel);
                var textSize = graphic.MeasureString(text, font);
                var color = Color.Blue;
                var brush = new SolidBrush(color);
                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));
                graphic.DrawString(text, font, brush, position);
                img.Save("wwwroot/Images/watermarks/" + imageCreatedEvent.ImageName);

                img.Dispose();
                graphic.Dispose();

                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
