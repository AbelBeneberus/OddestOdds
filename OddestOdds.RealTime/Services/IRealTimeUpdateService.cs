using OddestOdds.Common.Messages;

namespace OddestOdds.RealTime.Services;

public interface IRealTimeUpdateService<in T> where T : Message
{
    Task HandleMessageAsync(T message);
}