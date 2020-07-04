using InventoryService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.BackgroundServices.Requests
{
    public class GetAllRequestService : BackgroundService
    {
        private static readonly ConfigurationOptions _configuration = ConfigurationOptions.Parse("localhost:6379");
        private static readonly ConnectionMultiplexer _connection = ConnectionMultiplexer.Connect(_configuration);

        public IServiceProvider Services { get; }

        public GetAllRequestService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var scope = Services.CreateScope();
            var _requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();

            var database = _connection.GetDatabase();
            var Subscribe = _connection.GetSubscriber();

            var requests = await _requestService.GetAllRequest();
            var data = JsonConvert.SerializeObject(requests);

            await Subscribe.SubscribeAsync("get-all-requests", async (channel, message) =>
            {
                await Subscribe.PublishAsync("get-all-requests", data);
            });
        }
    }
}
