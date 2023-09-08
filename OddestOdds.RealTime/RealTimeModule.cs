using OddestOdds.Common.Messages;
using OddestOdds.RealTime.Services;

namespace OddestOdds.RealTime;

public static class RealTimeModule
{
    public static IServiceCollection AddRealTimeModule(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IRealTimeUpdateService<OddUpdatedMessage>, RealTimeUpdateService>();
        return services;
    }
}