using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway.RedisPubSub
{
    public interface IRabbitMQPubSub
    {
        Task<string> Handle(string queueName, string message ="", CancellationToken cancellationToken = default);
    }
}
