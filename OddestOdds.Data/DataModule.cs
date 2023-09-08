using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OddestOdds.Common.Configurations;
using OddestOdds.Data.Database;

namespace OddestOdds.Data;

public static class DataModule
{
    public static IServiceCollection AddDataModule(this IServiceCollection services,
        DatabaseConfiguration configuration)
    {
        services.AddDbContext<FixtureContext>(options => options.UseSqlServer(configuration.ConnectionString));
        return services;
    }
}