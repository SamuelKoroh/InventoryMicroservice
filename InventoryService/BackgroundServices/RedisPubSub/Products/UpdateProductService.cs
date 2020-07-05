using InventoryService.Domain.Models;
using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Products
{
    public class UpdateProductService : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public UpdateProductService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            await Subscribe.SubscribeAsync("update-product", async (channel, message) =>
            {
                var product = JsonConvert.DeserializeObject<Product>(Encoding.UTF8.GetString(message));
                var response = string.Empty;

                using var scope = Services.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                var productToUpdate = await productService.GetProductById(product.Id);

                if (productToUpdate == null)
                    response = "The product does not exists!";
                else
                {
                    await productService.UpdateProduct(productToUpdate, product);
                    response = "The product has been updated";
                }

                await Subscribe.PublishAsync("product-updated", response);
            });
        }
    }
}
