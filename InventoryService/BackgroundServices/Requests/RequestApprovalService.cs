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
    public class RequestApprovalService : BackgroundService
    {
        private static readonly ConfigurationOptions _configuration = ConfigurationOptions.Parse("localhost:6379");
        private static readonly ConnectionMultiplexer _connection = ConnectionMultiplexer.Connect(_configuration);

        public IServiceProvider Services { get; }

        public RequestApprovalService(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = _connection.GetDatabase();
            var Subscribe = _connection.GetSubscriber();

            await Subscribe.SubscribeAsync("approve-request", async (channel, message) =>
            {
                var request = JsonConvert.DeserializeObject<RequestApproval>(Encoding.UTF8.GetString(message));
                var response = string.Empty;

                using var scope = Services.CreateScope();
                var requestService = scope.ServiceProvider.GetRequiredService<IRequestService>();

                var requestToUpdate = await requestService.GetRequestById(request.RequestId);

                if (requestToUpdate == null)
                    response = "The request does not exists!";
                else
                {
                    var result =await requestService.UpdateRequest(requestToUpdate, request.IsApproved);
                    response = $"The request has been {result.Status}";
                }

                await Subscribe.PublishAsync("approve-request-response", response);
            });
        }
    }
}
