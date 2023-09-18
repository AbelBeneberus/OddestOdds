using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OddestOdds.Business.Services;
using OddestOdds.Business.Validations;
using OddestOdds.Common.Models;

namespace OddestOdds.Business;

public static class BusinessModule
{
    public static IServiceCollection AddBusinessModule(this IServiceCollection services)
    {
        services.AddScoped<IOddService, OddService>();
        services.AddTransient<IValidator<CreateOddRequest>, CreateOddRequestValidator>();
        services.AddTransient<IValidator<CreateFixtureRequest>, CreateFixtureRequestValidator>();
        services.AddTransient<IValidator<PushOddsRequest>, PushOddsRequestValidator>();
        return services;
    }
}