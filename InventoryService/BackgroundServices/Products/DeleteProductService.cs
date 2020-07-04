using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Products
{
    public class DeleteProductService : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public DeleteProductService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            await Subscribe.SubscribeAsync("delete-product", async (channel, message) =>
            {
                var id = Convert.ToInt32(Encoding.UTF8.GetString(message));
                var response = "";

                using var scope = Services.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                var product = await productService.GetProductById(id);

                if (product == null)
                    response = "The product does not exists!";
                else
                {
                    productService.DeleteProduct(product);
                    response = "The product has been deleted";
                }

                await Subscribe.PublishAsync("product-deleted", response);
            });

        }
    }
}
