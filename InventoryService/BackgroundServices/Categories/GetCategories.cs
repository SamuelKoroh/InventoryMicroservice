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
    public class GetCategories : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public GetCategories(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = Services.CreateScope();
            var _categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            var categories =  await _categoryService.GetCategories();
            var data = JsonConvert.SerializeObject(categories);

             await Subscribe.SubscribeAsync("get-categories", async (channel, message) =>
            {
                await Subscribe.PublishAsync("get-categories", data);
            });

        }
    }
}
