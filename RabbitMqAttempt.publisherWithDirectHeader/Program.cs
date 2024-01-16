using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ
{
    public class Program
    {
        enum LogNames
        {
            Critical = 1,
            Error = 2,
            Warning = 3,
            Information = 4
        }
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://isxqteoi:CwdH_6n-szCoOeh4T-4WkpiUBJ15GK3C@codfish.rmq.cloudamqp.com/isxqteoi");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel(); //kanal oluşturuldu


            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers); //Created a exchange

            Dictionary<string,object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            IBasicProperties properties=channel.CreateBasicProperties();
            properties.Headers=headers;

            channel.BasicPublish("header-exchange", string.Empty, properties,Encoding.UTF8.GetBytes("My Header Message"));


            Console.ReadLine();


            ///Exchange Types
            ///Header Exchange:Mesajların publisher tarafından header'da gönderilir header'lar üzerinden yakalanması
        }
    }
}