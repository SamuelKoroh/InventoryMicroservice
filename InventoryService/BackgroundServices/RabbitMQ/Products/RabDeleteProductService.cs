using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.RabbitMQ.Products
{
    public class RabDeleteProductService : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public IServiceProvider Services { get; }

        public RabDeleteProductService(IServiceProvider serviceProvider)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            Services = serviceProvider;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            channel.QueueDeclare(queue: "delete-product", durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "delete-product", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var response = string.Empty;
                var productId = Convert.ToInt32(Encoding.UTF8.GetString(body));

                using var scope = Services.CreateScope();

                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                var product = await productService.GetProductById(productId);

                if (product == null)
                    response = "The product does not exists!";
                else
                {
                    productService.DeleteProduct(product);
                    response = "The product has been deleted";
                }
                var responseBytes = Encoding.UTF8.GetBytes(response);

                channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                    basicProperties: replyProps, body: responseBytes);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            };

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            channel.Close();
            channel.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
