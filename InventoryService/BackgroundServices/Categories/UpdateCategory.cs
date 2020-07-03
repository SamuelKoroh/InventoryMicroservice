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

namespace InventoryService.BackgroundServices.Categories
{
    public class UpdateCategory : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public UpdateCategory(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            await Subscribe.SubscribeAsync("update-category", async (channel, message) =>
            {
                var category = JsonConvert.DeserializeObject<Category>(Encoding.UTF8.GetString(message));
                var response = string.Empty;

                using var scope = Services.CreateScope();
                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

                var categoryToUpdate = await categoryService.GetCategoryById(category.Id);

                if (categoryToUpdate == null)
                    response = "The category does not exists!";
                else
                {
                    await categoryService.UpdateCategory(categoryToUpdate, category);
                    response = "The category has been updated";
                }

                await Subscribe.PublishAsync("category-updated", response);
            });
        }
    }
}
