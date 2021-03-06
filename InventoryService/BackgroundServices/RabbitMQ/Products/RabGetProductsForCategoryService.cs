﻿using InventoryService.Domain.Services;
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
    public class RabGetProductsForCategoryService : BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public RabGetProductsForCategoryService(IServiceProvider service)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            Service = service;
        }

        public IServiceProvider Service { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            channel.QueueDeclare(queue: "get-products-by-category-id", durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "get-products-by-category-id", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
             {
                 var body = ea.Body.ToArray();
                 var props = ea.BasicProperties;
                 var replyProps = channel.CreateBasicProperties();
                 replyProps.CorrelationId = props.CorrelationId;

                 var categoryId = Convert.ToInt32(Encoding.UTF8.GetString(body));

                 using var scope = Service.CreateScope();
                 var _categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

                 var products = await _categoryService.GetProductsByCategoryId(categoryId);
                 var data = JsonConvert.SerializeObject(products);

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
