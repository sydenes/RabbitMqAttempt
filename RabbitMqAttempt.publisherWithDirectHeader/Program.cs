using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

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
            properties.Persistent = true; //Normalde durable ile kuyruklara kalıcılık sağlıyorduki Persistent ile mesajlara da kaıcılık sağladık. Diğer exchange'lerde de bu şekilde implement edilir.

            Product product = new() { Id = 1, Name = "Apple", Price=7.5m, Stock=65 }; //sadece string gönderebildiğimiz gibi bu şekilde nesneleri serialize/deserialize ederek gönderebiliriz. Ya da pdf, image vb dosyalarını binary olarak gönderebiliriz. Diğer exchangelerde de bu şekilde yapılabilir.
            var json=JsonSerializer.Serialize(product);

            channel.BasicPublish("header-exchange", string.Empty, properties,Encoding.UTF8.GetBytes(json));


            Console.ReadLine();


            ///Exchange Types
            ///Header Exchange:Mesajların publisher tarafından header'da gönderilir header'lar üzerinden yakalanması
        }
    }

    public class Product { public int Id; public string Name; public decimal Price; public int Stock; }
}