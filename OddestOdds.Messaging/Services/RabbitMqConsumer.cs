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
    private readonly IRealTimeUpdateService<OddCreatedMessage> _oddCreatedHandler;
    private readonly IRealTimeUpdateService<OddUpdatedMessage> _oddUpdatedHandler;
    private readonly ILogger<RabbitMqConsumer> _logger;

    private readonly string _queueName = "odd_manager";

    public RabbitMqConsumer(IConnection connection,
        ILogger<RabbitMqConsumer> logger,
        IRealTimeUpdateService<OddUpdatedMessage> oddUpdatedHandler,
        IRealTimeUpdateService<OddCreatedMessage> oddCreatedHandler)
    {
        _logger = logger;
        _oddUpdatedHandler = oddUpdatedHandler;
        _oddCreatedHandler = oddCreatedHandler;
        _model = connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_model);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var messageType = ea.BasicProperties.Type;

            switch (messageType)
            {
                case "OddCreated":
                    var oddCreatedMessage = JsonConvert.DeserializeObject<OddCreatedMessage>(message);
                    await _oddCreatedHandler.HandleMessageAsync(oddCreatedMessage);
                    break;
                case "OddUpdated":
                    var oddUpdatedMessage = JsonConvert.DeserializeObject<OddUpdatedMessage>(message);
                    await _oddUpdatedHandler.HandleMessageAsync(oddUpdatedMessage);
                    break;
            }
        };
        _model.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _model.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        await Task.Yield();
    }
}