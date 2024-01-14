using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ
{
    public class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://isxqteoi:CwdH_6n-szCoOeh4T-4WkpiUBJ15GK3C@codfish.rmq.cloudamqp.com/isxqteoi");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel(); //kanal oluşturuldu

            channel.QueueDeclare("hello-mq", true, false, false); //kuyruk tanımlandı.
                                                                  //durable: true olursa fiziksel kaydedilir Rmq resetlense bile tutulur, false'da kuyruklar memory'de tutulur res'de sıfırlanır.
                                                                  //Exclusive: true olursa sadace buradaki kanal üzerinden bağlanılabilir, false ile "subscriber" da oluşturulacak kanal üzerinden bağlanılacak.
                                                                  //AutoDelete: true'da subsribe bağlandıtı koptuğunda kuyruk silinir, false'da silinmez.

            for (int i = 0; i < 50; i++)
            {
                var msg = (i+1).ToString();

                var msgBody = Encoding.UTF8.GetBytes(msg);

                channel.BasicPublish(string.Empty, "hello-mq", null, msgBody); // şu anlık exchange kullanılmadığı için "string.Empty"

                Console.WriteLine(i+1+". Mesaj gönderildi.");
            }
            
            Console.ReadLine();

        }
    }
}