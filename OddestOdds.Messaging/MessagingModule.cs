using Microsoft.Extensions.DependencyInjection;
using OddestOdds.Common.Configurations;
using OddestOdds.Messaging.Services;
using RabbitMQ.Client;

namespace OddestOdds.Messaging;

public static class MessagingModule
{
    public static IServiceCollection AddMessagingModule(this IServiceCollection services,
        RabbitConfiguration configuration)
    {
        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration.HostName,
                UserName = configuration.UserName,
                Password = configuration.Password,
                Port = configuration.Port
            };

            return factory.CreateConnection();
        });
        services.AddSingleton<IMessagePublisherService, RabbitMqMessagePublisher>();
        services.AddHostedService<RabbitMqConsumer>();
        return services;
    }
}