﻿using InventoryService.Domain.Models;
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

namespace InventoryService.BackgroundServices.RabbitMQ.Products
{
    public class RabCreateProductService : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public IServiceProvider Services { get; }

        public RabCreateProductService(IServiceProvider serviceProvider)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            Services = serviceProvider;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            channel.QueueDeclare(queue: "create-product", durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "create-product", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var product = JsonConvert.DeserializeObject<Product>(Encoding.UTF8.GetString(body));
                var response = string.Empty;

                using var scope = Services.CreateScope();

                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                if (await productService.IsProductNameExisting(product))
                    response = "The product already exists!";
                else
                {
                    await productService.CreateProduct(product);
                    response = "The product was created successfully";
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
            channel.Dispose();
            connection.Dispose();
        }
    }
}
