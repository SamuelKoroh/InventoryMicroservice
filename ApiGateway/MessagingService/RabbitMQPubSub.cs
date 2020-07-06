using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway.RedisPubSub
{
    public class RabbitMQPubSub : IRabbitMQPubSub
    {

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly IBasicProperties _props;
        private readonly EventingBasicConsumer _consumer;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        public RabbitMQPubSub()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _replyQueueName = _channel.QueueDeclare().QueueName;
            _props = _channel.CreateBasicProperties();
            _props.ReplyTo = _replyQueueName;
            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<string> tcs))
                    return;

                tcs.TrySetResult(Encoding.UTF8.GetString(ea.Body.ToArray()));
            };
        }

        public Task<string> Handle(string queueName, string message, CancellationToken cancellationToken = default)
        {
            var correlationId = Guid.NewGuid().ToString();
            _props.CorrelationId = correlationId;

            var messageBytes = Encoding.UTF8.GetBytes(message);
            var tcs = new TaskCompletionSource<string>();
            _callbackMapper.TryAdd(correlationId, tcs);

            _channel.BasicPublish(exchange: "", routingKey: queueName,
                basicProperties: _props, body: messageBytes);

            _channel.BasicConsume(consumer: _consumer, queue: _replyQueueName, autoAck: true);

            cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out var tmp));
            return tcs.Task;
        }

        public void Dispose()
        {
            _channel.Close();
            _channel.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
