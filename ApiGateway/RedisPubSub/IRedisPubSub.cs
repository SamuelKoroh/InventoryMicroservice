using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway.RedisPubSub
{
    public interface IRedisPubSub
    {
        Task<TResponse> Handler<TData, TResponse>(TData data, string publishChannel, string subscribeChannel, CancellationToken cancellationToken = default);
        Task<object> Handler<TResponse>(string publishChannel, string subscribeChannel, CancellationToken cancellationToken = default);
    }
}
