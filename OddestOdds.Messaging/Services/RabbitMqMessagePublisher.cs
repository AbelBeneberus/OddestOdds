using System.Text;
using Newtonsoft.Json;
using OddestOdds.Common.Messages;
using RabbitMQ.Client;

namespace OddestOdds.Messaging.Services
{
    public class RabbitMqMessagePublisher : IMessagePublisherService, IDisposable
    {
        private readonly string _exchangeName = "odds_exchange";
        private readonly IConnection _connection;

        public RabbitMqMessagePublisher(IConnection connection)
        {
            _connection = connection;
        }

        public Task PublishMessageAsync(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            using var channel = _connection.CreateModel();

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var serializedMessage = JsonConvert.SerializeObject(message, settings);

            var body = Encoding.UTF8.GetBytes(serializedMessage);

            var routingKey = "";

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Type = message.Type;

            channel.BasicPublish(exchange: _exchangeName, routingKey: routingKey, basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}