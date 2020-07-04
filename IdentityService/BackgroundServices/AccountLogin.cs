using IdentityService.Domain.Services;
using IdentityService.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService.BackgroundServices
{
    public class AccountLogin : BackgroundService
    {
        private static readonly ConfigurationOptions _configuration = ConfigurationOptions.Parse("localhost:6379");
        private static readonly ConnectionMultiplexer _connection = ConnectionMultiplexer.Connect(_configuration);

        public IServiceProvider Services { get; }

        public AccountLogin(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = _connection.GetDatabase();
            var Subscribe = _connection.GetSubscriber();

            await Subscribe.SubscribeAsync("login-request", async (channel, message) =>
            {
                var loginResource = JsonConvert.DeserializeObject<LoginResource>(Encoding.UTF8.GetString(message));

                using var scope = Services.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                var response = await authService.LoginAsync(loginResource);

                await Subscribe.PublishAsync("login-response", response);
            });
        }
    }
}
