

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

            //channel.QueueDeclare("hello-mq", true, false, false);//publisher'ın bu isimde bir kuyruk oluşturduğundan eminsek burayı silebiliriz ancak oluşturmazsa burası kuyruğu oluşturur diğer türlü hata alırız.

            //channel.BasicQos(0,6,false); //0: herhangi bir boyutta mesaj gelebilir. 6 ve false olması örneğin 2 subscriber varsa ilk gelen 6 mesajı 1. subs'a diğer 6'yı 2. subs'a şeklinde. Eğer true olursa; 3-3 paylaştırır.
            channel.BasicQos(0,1,false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-mq", false,consumer); //AutoAck: true olduğunda, mesaj doğruda işlense yanlışda işlense kuyruktan siler. false olursa silmez başarılı olursa ayrıca biz silicez.

            //consumer.Received += Consumer_Received; //publisher bu consumer'a(subscriber) mesaj gönderdiğinde bu event fırlayacak. ayrıca metot olarak tanımlanacağı gibi lamda ile de tanımlanabilir.
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Mesajınız:"+msg);

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