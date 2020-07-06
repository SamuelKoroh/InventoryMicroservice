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

namespace InventoryService.BackgroundServices.RabbitMQ.Requests
{
    public class RabGetAllRequestByRequesterService : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public IServiceProvider Services { get; }

        public RabGetAllRequestByRequesterService(IServiceProvider serviceProvider)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            Services = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            channel.QueueDeclare(queue: "get-my-requests", durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "get-my-requests", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var requesterId = Encoding.UTF8.GetString(body);

                using var scope = Services.CreateScope();

                var requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();
                var requests = await requestService.MyRequests(requesterId);
                var data = JsonConvert.SerializeObject(requests);
                var responseBytes = Encoding.UTF8.GetBytes(data);

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
