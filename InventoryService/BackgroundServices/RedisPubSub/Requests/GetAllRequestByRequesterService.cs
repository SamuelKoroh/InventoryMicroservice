using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Requests
{
    public class GetAllRequestByRequesterService : BackgroundService
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);

        public IServiceProvider Services { get; }

        public GetAllRequestByRequesterService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = connection.GetDatabase();
            var Subscribe = connection.GetSubscriber();

            await Subscribe.SubscribeAsync("get-my-requests", async (channel, message) =>
            {
                using var scope = Services.CreateScope();
                var _requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();

                var requesterId = Encoding.UTF8.GetString(message);

                var requests = await _requestService.MyRequests(requesterId);
                var data = JsonConvert.SerializeObject(requests);

                await Subscribe.PublishAsync("get-my-requests-reply", data);
            });
        }
    }
}
