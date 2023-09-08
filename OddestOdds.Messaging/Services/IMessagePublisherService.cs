using OddestOdds.Common.Messages;

namespace OddestOdds.Messaging.Services;

public interface IMessagePublisherService
{
    Task PublishMessageAsync(Message message);
}