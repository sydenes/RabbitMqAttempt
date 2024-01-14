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


            //Queue'lar artık publisher yerine exchange üzerinden yönetileceği için queue decleration'ı burada yoruma alıp exchange tanımladık.
            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout); // durable:true olduğunda uygulama restart olduğunda exchange'Lerin kaybolmaması için.

            for (int i = 0; i < 50; i++)
            {
                var msg = (i + 1).ToString();

                var msgBody = Encoding.UTF8.GetBytes(msg);

                channel.BasicPublish("logs-fanout", "", null, msgBody);

                Console.WriteLine(i + 1 + ". Mesaj gönderildi.");
            }

            Console.ReadLine();


            ///Exchange Types
            ///Fanout Exchange: Producer'dan gelen mesajı exchange bağlı olan tüm queue'lara filtreden geçirmeden aynı mesajı gönderir. Fanout exchange'de genelde consumer'lar kendi kuyruklarını kendileri oluşturarak fanout exchange'e bağlanırlar. Örneğin bir hava durumu veya borsa verilerinde fanout exchange oluşturulup buradan data çekip beslenmek isteyen consumer projeler bu fanout exchange'e queue oluşturup bağlanılar. Queue'lar producer tarafından oluşturulmaz.
            ///
        }
    }
}