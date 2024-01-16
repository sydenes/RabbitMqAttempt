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


            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic); //Created a exchange


            for (int i = 0; i < 50; i++)
            {
                Random rnd = new Random();

                LogNames log1 = (LogNames)rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);

                var routeKey = $"{log1}.{log2}.{log3}";

                var msg = $"Log-Message-Type: {log1}-{log2}-{log3}";

                var msgBody = Encoding.UTF8.GetBytes(msg);


                channel.BasicPublish("logs-topic", routeKey, null, msgBody);

                Console.WriteLine($"Log send: {msg}");
            }

            Console.ReadLine();


            ///Exchange Types
            ///Topic Exchange: Queue'ların oluşturulma işlemi consumerlar tarfından yaılır çünkü çok fazla varsayon vardır. Örneğin: Critical.Error.Warning şeklinde publish edilmiş bir mesaj
            /// *.Error.* : Ortası Error olanlar
            /// Critical.# : Critical ile başlayanlar
            /// *.*.Warning : Warning ile bitenler
            /// şeklinde tanımlanmış 3 kuyruğa da düşecektir.
        }
    }
}