using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway.RedisPubSub
{
    public interface IRedisPubSub
    {
        Task<object> HandleAndReturnMessage(string publishChannel, string subscribeChannel, object data, bool serializeData = false, CancellationToken cancellationToken = default);
        Task<object> HandleAndDeserialize<TDeserializeObject>(string publishChannel, string subscribeChannel, string messageData = "", CancellationToken cancellationToken = default);
    }
}
