using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ
{
    public class Program
    {
        enum LogNames
        {
            Critical=1,
            Error=2,
            Warning=3,
            Information=4
        }
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://isxqteoi:CwdH_6n-szCoOeh4T-4WkpiUBJ15GK3C@codfish.rmq.cloudamqp.com/isxqteoi");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel(); //kanal oluşturuldu


            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct); //Created a exchange

            Enum.GetNames<LogNames>().ToList().ForEach(name =>
            {
                var queueName = $"direct-queue{name}";

                var routeKey = $"route-{name}"; //Publisher'dan basılacak mesajın hangi route'a gideceğini belirten key.

                channel.QueueDeclare(queueName, true, false, false); //Declerated a queue

                channel.QueueBind(queueName, "logs-direct", routeKey); // The queue binded to created exchange

            });

            for (int i = 0; i < 50; i++)
            {
                var log = (LogNames)new Random().Next(1, 5);

                var msg = $"{i+1}. Log-Message-Type: {log}";

                var msgBody = Encoding.UTF8.GetBytes(msg);

                var routeKey = $"route-{log}"; //Mesajın publish edileceği queueBinding'in key'i

                channel.BasicPublish("logs-direct", routeKey, null, msgBody);

                Console.WriteLine($"Log send: {msg}");
            }

            Console.ReadLine();


            ///Exchange Types
            ///Direct Exchange: Queue'ların önceden belirlenerek producer tarafında oluşturulmasıdır. İhtiyaca göre "route-key"ler ile yapılabilecek işlemlerin queue'ları oluşturulup ilgili mesajlar buralara gönderilir. Daha sonrasında Consumerlar ihtiyacı olan ilgili queu'ya bağlanarak bu mesajları alırlar.
            ///Örneğin: Critical, Error, Warning, Info bilgilerini ayrı ortamlara yazacağımız servisler var bunun için producer tarafında 4 tane queue oluşturulup consumerlar ihtiyacı olanlara bağlanır. Bu yüzden queue'lar kalıcı oluşturulur.
        }
    }
}