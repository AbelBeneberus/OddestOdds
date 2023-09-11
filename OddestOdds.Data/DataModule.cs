using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OddestOdds.Data.Configuration;
using OddestOdds.Data.Database;
using OddestOdds.Data.Repository;

namespace OddestOdds.Data;

public static class DataModule
{
    public static IServiceCollection AddDataModule(this IServiceCollection services,
        DatabaseConfiguration configuration)
    {
        services.AddDbContext<FixtureDbContext>(options => options.UseSqlServer(configuration.ConnectionString));
        services.AddScoped<IFixtureRepository, FixtureRepository>();
        return services;
    }
}