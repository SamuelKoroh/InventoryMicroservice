using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway.RedisPubSub
{
    public class RedisPubSubHandler : IRedisPubSub
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);
        private static ConcurrentDictionary<string, TaskCompletionSource<object>> callBackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<object>>();
        public Task<TResponse> Handler<TData, TResponse>(TData data, string publishChannel, string subscribeChannel, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public  Task<object> Handler<TResponse>(string publishChannel, string subscribeChannel, CancellationToken cancellationToken = default)
        {
            var database = connection.GetDatabase();
            var publisher = connection.GetSubscriber();
            var tcs = new TaskCompletionSource<object>();

            publisher.Publish(publishChannel, "");

            publisher.Subscribe(subscribeChannel, (channel, message) =>
            {
                var result = JsonConvert.DeserializeObject<TResponse>(message);

                tcs.TrySetResult(result);
            });

            callBackMapper.TryAdd(subscribeChannel, tcs);


            return tcs.Task;
        }
    }
}
