using InventoryService.Domain.Models;
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
    public class RabCreateCategoryService : BackgroundService
    {
        public IServiceProvider Services { get; }

        public RabCreateCategoryService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "create-category", durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "create-category", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var category = JsonConvert.DeserializeObject<Category>(Encoding.UTF8.GetString(body));
                var response = string.Empty;

                using var scope = Services.CreateScope();

                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

                if (await categoryService.IsCategoryNameExisting(category))
                    response = "The category already exists!";
                else
                {
                    await categoryService.CreateCategory(category);
                    response = "The category was created successfully";
                }

                var responseBytes = Encoding.UTF8.GetBytes(response);

                channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                    basicProperties: replyProps, body: responseBytes);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            };

            return Task.CompletedTask;
        }
    }
}
