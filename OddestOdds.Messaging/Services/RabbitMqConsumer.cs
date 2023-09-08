using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OddestOdds.Common.Messages;
using OddestOdds.RealTime.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OddestOdds.Messaging.Services;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IModel _model;
    private readonly IRealTimeUpdateService<OddUpdatedMessage> _realTimeUpdateService;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private readonly string _queueName = "odd_manager";

    public RabbitMqConsumer(IConnection connection,
        IRealTimeUpdateService<OddUpdatedMessage> realTimeUpdateService,
        ILogger<RabbitMqConsumer> logger)
    {
        _realTimeUpdateService = realTimeUpdateService;
        _logger = logger;
        _model = connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_model);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var oddUpdatedMessage = JsonConvert.DeserializeObject<OddUpdatedMessage>(message);
            await _realTimeUpdateService.SendOddsUpdateAsync(oddUpdatedMessage);
        };
        _model.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _model.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        await Task.Yield();
    }
}