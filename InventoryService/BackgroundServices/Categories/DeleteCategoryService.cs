using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Categories
{
    public class DeleteCategoryService : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public DeleteCategoryService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            await Subscribe.SubscribeAsync("delete-category", async (channel, message) =>
            {
                var id = Convert.ToInt32(Encoding.UTF8.GetString(message));
                var response = "";

                using var scope = Services.CreateScope();
                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

                var category = await categoryService.GetCategoryById(id);

                if (category == null)
                    response = "The category does not exists!";
                else
                {
                    categoryService.DeleteCategory(category);
                    response = "The category has been deleted";
                }

                await Subscribe.PublishAsync("category-deleted", response);
            });

        }
    }
}
