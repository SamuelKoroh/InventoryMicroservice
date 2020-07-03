using ApiGateway.Domain.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway.RedisPubSub
{
    public class GenericPusSub<T>
    {
        private static ConfigurationOptions configuration = ConfigurationOptions.Parse("localhost:6379");
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(configuration);
        private static ConcurrentDictionary<string, TaskCompletionSource<T>> callbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<T>>();

        public static Task<T> GetGeneric(string channelName, string messageData ="",CancellationToken cancellationToken = default(CancellationToken))
        {
            var database = connection.GetDatabase();
            var publisher = connection.GetSubscriber();
            var tcs = new TaskCompletionSource<T>();

            publisher.Publish("channelName", messageData);

            publisher.Subscribe("channelName", (channel, message) =>
            {
                var result = JsonConvert.DeserializeObject<T>(message);

                tcs.TrySetResult(result);
            });

            callbackMapper.TryAdd("channelName", tcs);


            return tcs.Task;
        }
    }
}
