using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace OddestOdds.Messaging.Helper;

public class RabbitMqTopologyCreator
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private readonly int _port;
    private readonly string _exchangeName = "odds_exchange";

    public RabbitMqTopologyCreator(string hostName, string userName, string password, int port)
    {
        _hostName = hostName ?? throw new ArgumentNullException(nameof(hostName));
        _userName = userName ?? throw new ArgumentNullException(nameof(userName));
        _password = password ?? throw new ArgumentNullException(nameof(password));
        _port = port;
    }

    public void Setup()
    {
        var factory = new ConnectionFactory
            { HostName = _hostName, UserName = _userName, Password = _password, Port = _port };

        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the exchange if not exists
            channel.ExchangeDeclarePassive(_exchangeName);
        }
        catch (Exception ex)
        {
            if (ex is OperationInterruptedException)
            {
                // Exchange doesn't exist. We should declare it.
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout);
            }
            else
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        var queues = new[] { "odd_manager" };
        foreach (var queue in queues)
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            try
            {
                channel.QueueBind(queue: queue, exchange: _exchangeName, routingKey: "");
            }
            catch
            {
                Console.WriteLine($"Failed to bind queue: {queue}");
            }
        }
    }
}