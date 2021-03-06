﻿using IdentityService.Domain.Services;
using IdentityService.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService.BackgroundServices
{
    public class AccountLoginService : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public IServiceProvider Services { get; }

        public AccountLoginService(IServiceProvider serviceProvider)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            Services = serviceProvider;
        }
        protected override  Task ExecuteAsync(CancellationToken stoppingToken)
        {
            channel.QueueDeclare(queue: "login-request", durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "login-request", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var loginResource = JsonConvert.DeserializeObject<LoginResource>(Encoding.UTF8.GetString(body));

                using var scope = Services.CreateScope();

                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                var response = await authService.LoginAsync(loginResource);

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
