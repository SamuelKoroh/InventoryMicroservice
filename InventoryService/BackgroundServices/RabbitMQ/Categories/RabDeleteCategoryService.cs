using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.RabbitMQ.Categories
{
    public class RabDeleteCategoryService : BackgroundService
    {
        public IServiceProvider Services { get; }

        public RabDeleteCategoryService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "delete-category", durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "delete-category", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var response = string.Empty;
                var categoryId = Convert.ToInt32(Encoding.UTF8.GetString(body));

                using var scope = Services.CreateScope();

                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();
                var category = await categoryService.GetCategoryById(categoryId);

                if (category == null)
                    response = "The category does not exists!";
                else
                {
                    categoryService.DeleteCategory(category);
                    response = "The category has been deleted";
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
