using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.RabbitMQ.Categories
{
    public class RabGetAllCategoryService : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public IServiceProvider Services { get; }

        public RabGetAllCategoryService(IServiceProvider service)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            Services = service;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            channel.QueueDeclare(queue: "get-all-category", durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "get-all-category", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
             {
                 var body = ea.Body.ToArray();
                 var props = ea.BasicProperties;
                 var replyProps = channel.CreateBasicProperties();
                 replyProps.CorrelationId = props.CorrelationId;


                 using var scope = Services.CreateScope();
                 var _categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

                 var categories = await _categoryService.GetCategories();
                 var data = JsonConvert.SerializeObject(categories);

                 var responseBytes = Encoding.UTF8.GetBytes(data);

                 channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                     basicProperties: replyProps, body: responseBytes);
                 channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

             };

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }
    }
}
