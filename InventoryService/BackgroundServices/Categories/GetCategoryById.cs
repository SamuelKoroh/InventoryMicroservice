using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Categories
{
    public class GetCategoryById : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public GetCategoryById(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            await Subscribe.SubscribeAsync("get-category-by-id", async (channel, message) =>
            {
                var id = Convert.ToInt32(Encoding.UTF8.GetString(message));

                using var scope = Services.CreateScope();
                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();
                var category = await categoryService.GetCategoryById(id);

                var data = JsonConvert.SerializeObject(category);

                await Subscribe.PublishAsync("get-category-by-id-reply", data);
            });

        }
    }
}
