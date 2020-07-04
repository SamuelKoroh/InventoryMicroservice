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
    public class GetProductsByCategoryIdService : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public GetProductsByCategoryIdService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            await Subscribe.SubscribeAsync("get-products-by-category-id", async (channel, message) =>
            {
                var categoryId = Convert.ToInt32(Encoding.UTF8.GetString(message));
                using var scope = Services.CreateScope();
                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();


                var products = await categoryService.GetProductsByCategoryId(categoryId);
                var data = JsonConvert.SerializeObject(products);

                await Subscribe.PublishAsync("get-products-by-category-id-reply", data);
            });
        }
    }
}
