using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitmqWatermark.Services
{
    public class RabbitMqPublisher
    {
        private readonly RabbitMqClientService _rabbitMqClientService;
        public RabbitMqPublisher(RabbitMqClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }
        public void Publish(productImageCreatedEvent productImageCreatedEvent)
        {
            var channel=_rabbitMqClientService.Connect();

            var bodyString=JsonSerializer.Serialize(productImageCreatedEvent);
            var bodyByte=Encoding.UTF8.GetBytes(bodyString);
            var properties=channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(RabbitMqClientService.ExchangeName, RabbitMqClientService.RoutingWatermark, basicProperties: properties, body: bodyByte);
        }
    }
}
