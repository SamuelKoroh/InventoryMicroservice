using InventoryService.Domain.Models;
using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Categories
{
    public class CreateCategory : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public CreateCategory(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            await Subscribe.SubscribeAsync("create-category", async (channel, message) =>
            {
                var category = JsonConvert.DeserializeObject<Category>(Encoding.UTF8.GetString(message));
                var response = string.Empty;

                using var scope = Services.CreateScope();
                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

                if (await categoryService.IsCategoryNameExisting(category))
                    response = "The category already exists!";
                else
                {
                    await categoryService.CreateCategory(category);
                    response = "The category was created successfully";
                }

                await Subscribe.PublishAsync("category-created", response);
            });
        }
    }
}
