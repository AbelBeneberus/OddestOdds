using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using OddestOdds.Caching;
using OddestOdds.Common.Configurations;
using OddestOdds.Data;
using OddestOdds.Data.Database;
using OddestOdds.Messaging;
using OddestOdds.RealTime;

namespace OddestOdds.HandlerApp;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddMessagingModule(Configuration.GetSection("Rabbit").Get<RabbitConfiguration>());
        services.AddCachingModule(Configuration.GetSection("Redis").Get<RedisConfiguration>());
        services.AddDataModule(Configuration.GetSection("Database").Get<DatabaseConfiguration>());
        services.AddRealTimeModule();
        services.AddHealthChecks()
            .AddCheck("Ping", () => HealthCheckResult
                    .Healthy("Ping is OK."),
                tags: new[] { "ping" })
            .AddDbContextCheck<FixtureContext>(
                "Database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "sql" }
            );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<FixtureContext>();
            context.Database.Migrate();
        }

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(
                        new
                        {
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(entry => new
                            {
                                name = entry.Key,
                                status = entry.Value.Status.ToString(),
                                description = GetHealthCheckDescription(entry)
                            })
                        },
                        Formatting.Indented
                    );
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });
        });
    }

    private string GetHealthCheckDescription(KeyValuePair<string, HealthReportEntry> entry)
    {
        if (entry.Key == "Database")
        {
            return entry.Value.Status == HealthStatus.Healthy
                ? "Database is OK"
                : "Connection to SQL database failed.";
        }

        return entry.Value.Description;
    }
}