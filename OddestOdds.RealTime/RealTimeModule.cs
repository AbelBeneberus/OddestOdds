﻿using OddestOdds.Common.Messages;
using OddestOdds.RealTime.Services;

namespace OddestOdds.RealTime;

public static class RealTimeModule
{
    public static IServiceCollection AddRealTimeModule(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IRealTimeUpdateService<OddUpdatedMessage>, RealTimeUpdateService>();
        services.AddSingleton<IRealTimeUpdateService<OddCreatedMessage>, RealTimeUpdateService>();
        services.AddSingleton<IRealTimeUpdateService<OddDeletedMessage>, RealTimeUpdateService>(); 
        services.AddSingleton<IRealTimeUpdateService<PushedOddMessage>, RealTimeUpdateService>(); 
        return services;
    }
}