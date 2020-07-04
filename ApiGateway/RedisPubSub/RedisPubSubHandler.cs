using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text;
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

        public Task<object> HandleAndReturnMessage(string publishChannel, string subscribeChannel, object data, bool serializeData = false, CancellationToken cancellationToken = default)
        {
            var database = connection.GetDatabase();
            var publisher = connection.GetSubscriber();
            var tcs = new TaskCompletionSource<object>();

            var messageData = serializeData ? JsonConvert.SerializeObject(data) : data.ToString();

            publisher.Publish(publishChannel, messageData);

            publisher.Subscribe(subscribeChannel, (channel, message) =>
            {
                tcs.TrySetResult(Encoding.UTF8.GetString(message));
            });

            callBackMapper.TryAdd(subscribeChannel, tcs);

            return tcs.Task;
        }

        public Task<object> HandleAndDeserialize<TDeserializeObject>(string publishChannel, string subscribeChannel, string messageData = "", CancellationToken cancellationToken = default)
        {
            var database = connection.GetDatabase();
            var publisher = connection.GetSubscriber();
            var tcs = new TaskCompletionSource<object>();

            publisher.Publish(publishChannel, messageData);

            publisher.Subscribe(subscribeChannel, (channel, message) =>
            {
                var result = JsonConvert.DeserializeObject<TDeserializeObject>(message);

                tcs.TrySetResult(result);
            });

            callBackMapper.TryAdd(subscribeChannel, tcs);


            return tcs.Task;
        }
    }
}
