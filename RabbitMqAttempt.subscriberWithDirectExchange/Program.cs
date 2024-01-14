
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.subscriber
{
    public class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://isxqteoi:CwdH_6n-szCoOeh4T-4WkpiUBJ15GK3C@codfish.rmq.cloudamqp.com/isxqteoi");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel(); //kanal oluşturuldu

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            Console.WriteLine("Listening logs...");

            var queueName = "direct-queueCritical";
            channel.BasicConsume(queueName, false, consumer); //Queue declare ve Binding işlemleri publisher tarafında yapıldığı için doğrudan mevcut queu'ya bağlanarak consume edildi.

            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(500);
                Console.WriteLine("Mesajınız:" + msg);

                File.AppendAllText("log-critical.txt", msg+"\n");

                channel.BasicAck(e.DeliveryTag, false); //RabbitMq mesajı gönderdiğinde burdan gelecek cevabı bekler cevap gelmediğinde mesajı silmeyip varsa başka bir sucscriber instance'ına gönderir.
            };

            Console.ReadLine();
        }

        private static void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}