using InventoryService.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Products
{
    public class GetProducts : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public GetProducts(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var scope = Services.CreateScope();
            var _productService = scope.ServiceProvider.GetRequiredService <IProductService>();

            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();


            var products =  await _productService.GetProducts();
            var data = JsonConvert.SerializeObject(products);

             await Subscribe.SubscribeAsync("get-products", async (channel, message) =>
            {
                await Subscribe.PublishAsync("get-products-reply", data);
            });
        }
    }
}
