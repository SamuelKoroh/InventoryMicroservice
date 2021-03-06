﻿using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Products
{
    public class GetAvailableProductsService : BackgroundService
    {
        private static readonly ConfigurationOptions _configuration = ConfigurationOptions.Parse("localhost:6379");
        private static readonly ConnectionMultiplexer _connection = ConnectionMultiplexer.Connect(_configuration);

        public IServiceProvider Services { get; }

        public GetAvailableProductsService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var scope = Services.CreateScope();
            var _productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            var database = _connection.GetDatabase();
            var Subscribe = _connection.GetSubscriber();


            var products = await _productService.GetProducts(true);
            var data = JsonConvert.SerializeObject(products);

            await Subscribe.SubscribeAsync("get-available-products", async (channel, message) =>
            {
                await Subscribe.PublishAsync("get-available-products", data);
            });
        }
    }
}
