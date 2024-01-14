
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

            var queueName = "log-database-save"; //channel.QueueDeclare().QueueName;

            //channel.QueueDeclare(queueName,true,false,false);
            channel.QueueBind(queueName, "logs-fanout", "", null); //Subscriber down olduğunda silinmesini istediğimiz için queue "declerate" etmek yerine "bind" ettik.

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            Console.WriteLine("Listening logs...");

            channel.BasicConsume(queueName, false, consumer); 

            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1000);
                Console.WriteLine("Mesajınız:" + msg);

                channel.BasicAck(e.DeliveryTag, false); //BasicAck: gelen mesajı rabitMq'dan siler. False olduğunda sadece ilgili mesajın bilgisini rabbitMq'ya verir, true olduğunda kendinden sonraki mesaj için de bilgi döner.
            };

            Console.ReadLine();
        }

        private static void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}