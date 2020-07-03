using InventoryService.Domain.Models;
using InventoryService.Domain.Services;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Categories
{
    public class CategoriesBackgroundService : BackgroundService
    {
        //private readonly ICategoryService _categoryService;

        //public CategoriesBackgroundService(ICategoryService categoryService)
        //{
        //    _categoryService = categoryService;
        //}
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "categories_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);

                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);

                channel.BasicConsume(queue: "categories_queue",
                  autoAck: false, consumer: consumer);

                Console.WriteLine(" [x] Awaiting RPC requests");
                var categories = new List<Category> {
                    new Category{ Id = 1, Name =" category 1"},
                    new Category{ Id = 2, Name =" category 2"},
                    new Category{ Id = 3, Name =" category 3"},
                };

                consumer.Received += (model, ea) =>
                {
                    string response = null;
                    
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        response = JsonConvert.SerializeObject(categories);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };

                return Task.CompletedTask;
            }
        }
    }
}
