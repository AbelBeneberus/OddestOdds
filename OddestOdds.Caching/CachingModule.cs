using Microsoft.Extensions.DependencyInjection;
using OddestOdds.Caching.Repositories;
using OddestOdds.Caching.Services;
using OddestOdds.Common.Configurations;

namespace OddestOdds.Caching;

public static class CachingModule
{
    public static IServiceCollection AddCachingModule(this IServiceCollection services,
        RedisConfiguration configuration)
    {
        services.AddSingleton<IRedisService>(new RedisService(configuration.Host));
        services.AddScoped<ICacheRepository, RedisCacheRepository>();
        return services;
    }
}