using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.Domain;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Guid process = Guid.NewGuid();
            System.Threading.Thread threadSender = new System.Threading.Thread(() =>
            {

                while (true)
                {
                    MensagemDto content = new MensagemDto(process);
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "qapp",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var message = JsonSerializer.Serialize(content);
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "qapp",
                                             basicProperties: null,
                                             body: body);
                        Console.WriteLine("Sent {0}", message);
                        Console.WriteLine("");
                    }
                    Thread.Sleep(5000);
                }
            });
            threadSender.Name = string.Concat("threadSender");
            threadSender.Start();
            System.Threading.Thread.Sleep(100);



            System.Threading.Thread threadReceiver = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "qapp",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            
                            
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            var result = JsonSerializer.Deserialize<MensagemDto>(message);

                            if (process != result.Processo)
                            { 
                                Console.WriteLine("Received {0} From Process {1} RequestId {2}", result.Mensagem, result.Processo, result.Identificador);
                                Console.WriteLine("");
                            }



                        };
                        channel.BasicConsume(queue: "qapp",
                                             autoAck: true,
                                             consumer: consumer);

                        Thread.Sleep(5000);
                    }
                }
            });
            threadReceiver.Name = string.Concat("threadReceiver");
            threadReceiver.Start();
            System.Threading.Thread.Sleep(100);


        }

    }
}
