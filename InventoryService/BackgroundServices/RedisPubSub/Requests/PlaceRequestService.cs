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

namespace InventoryService.BackgroundServices.Requests
{
    public class PlaceRequestService : BackgroundService
    {
        private static readonly ConfigurationOptions _configuration = ConfigurationOptions.Parse("localhost:6379");
        private static readonly ConnectionMultiplexer _connection = ConnectionMultiplexer.Connect(_configuration);

        public IServiceProvider Services { get; }

        public PlaceRequestService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = _connection.GetDatabase();
            var Subscribe = _connection.GetSubscriber();

            await Subscribe.SubscribeAsync("place-request", async (channel, message) =>
            {
                var request = JsonConvert.DeserializeObject<Request>(Encoding.UTF8.GetString(message));
                var response = string.Empty;

                using var scope = Services.CreateScope();
                var requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();

                try
                {
                    await requestService.MakeRequest(request);
                    response = "Your request has been placed successfully";
                }
                catch (Exception ex)
                {
                    response = ex.Message;
                }
                
                await Subscribe.PublishAsync("place-request-response", response);
            });
        }
    }
}
