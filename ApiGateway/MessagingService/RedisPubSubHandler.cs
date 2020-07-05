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
        private static readonly ConfigurationOptions _configuration = ConfigurationOptions.Parse("localhost:6379");
        private static readonly ConnectionMultiplexer _connection = ConnectionMultiplexer.Connect(_configuration);
        private static readonly ConcurrentDictionary<string, TaskCompletionSource<object>> _callBackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<object>>();

        public Task<object> HandleAndReturnMessage(string publishChannel, string subscribeChannel, object data, bool serializeData = false, CancellationToken cancellationToken = default)
        {
            var database = _connection.GetDatabase();
            var publisher = _connection.GetSubscriber();
            var tcs = new TaskCompletionSource<object>();

            var messageData = serializeData ? JsonConvert.SerializeObject(data) : data.ToString();

            publisher.Publish(publishChannel, messageData);

            publisher.Subscribe(subscribeChannel, (channel, message) =>
            {
                tcs.TrySetResult(Encoding.UTF8.GetString(message));
            });

            _callBackMapper.TryAdd(subscribeChannel, tcs);

            return tcs.Task;
        }

        public Task<object> HandleAndDeserialize<TDeserializeObject>(string publishChannel, string subscribeChannel, string messageData = "", CancellationToken cancellationToken = default)
        {
            var database = _connection.GetDatabase();
            var publisher = _connection.GetSubscriber();
            var tcs = new TaskCompletionSource<object>();

            publisher.Publish(publishChannel, messageData);

            publisher.Subscribe(subscribeChannel, (channel, message) =>
            {
                var result = JsonConvert.DeserializeObject<TDeserializeObject>(message);

                tcs.TrySetResult(result);
            });

            _callBackMapper.TryAdd(subscribeChannel, tcs);


            return tcs.Task;
        }
    }
}
