﻿
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

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

            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string,object> headers= new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "all"); //all: header'daki tüm alanlar eşleşmeli, any: herhangi biri eşleşmesi yeterli

            channel.QueueBind(queueName, "header-exchange",string.Empty,headers);

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Listening logs...");


            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                Product product= JsonSerializer.Deserialize<Product>(msg);

                Thread.Sleep(500);
                Console.WriteLine($"Mesajınız:{product.Id}-{product.Name}-{product.Price}-{product.Stock}-");

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }

        private static void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            throw new NotImplementedException();
        }

        public class Product { public int Id; public string Name; public decimal Price; public int Stock; }
    }
}