using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using OddestOdds.Business;
using OddestOdds.Caching;
using OddestOdds.Common.Configurations;
using OddestOdds.Data;
using OddestOdds.Data.Configuration;
using OddestOdds.Data.Database;
using OddestOdds.Messaging;
using OddestOdds.RealTime;
using Swashbuckle.AspNetCore.Filters;

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
        var redisConfiguration = Configuration.GetSection("Redis").Get<RedisConfiguration>();
        var rabbitConfiguration = Configuration.GetSection("Rabbit").Get<RabbitConfiguration>();
        services.AddControllers();
        services.AddMessagingModule(rabbitConfiguration);
        services.AddCachingModule(redisConfiguration);
        services.AddDataModule(Configuration.GetSection("Database").Get<DatabaseConfiguration>());
        services.AddRealTimeModule();
        services.AddBusinessModule();
        services.AddHealthChecks()
            .AddCheck("Ping", () => HealthCheckResult
                    .Healthy("Ping is OK."),
                tags: new[] { "ping" })
            .AddDbContextCheck<FixtureDbContext>(
                "Database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "sql" })
            .AddRedis(redisConnectionString: redisConfiguration.Host, name: "Redis")
            .AddRabbitMQ(
                $"amqp://{rabbitConfiguration.UserName}:{rabbitConfiguration.Password}@{rabbitConfiguration.HostName}:{rabbitConfiguration.Port}",
                name: "RabbitMQ",
                failureStatus: HealthStatus.Degraded,
                timeout: TimeSpan.FromSeconds(1));

        services.AddSwaggerGen(options => { options.ExampleFilters(); });
        services.AddSwaggerExamplesFromAssemblyOf<Program>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<FixtureDbContext>();
            context.Database.Migrate();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "OddValue Handler Service V1"); });

        app.UseRouting();
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
            endpoints.MapHub<MessageHub>("/messageHub");
        });
    }

    private string GetHealthCheckDescription(KeyValuePair<string, HealthReportEntry> entry)
    {
        switch (entry.Key)
        {
            case "Database":
                return entry.Value.Status == HealthStatus.Healthy
                    ? "Database is OK"
                    : "Connection to SQL database failed.";
                break;
            case "Redis":
                return entry.Value.Status == HealthStatus.Healthy
                    ? "Redis is OK"
                    : "Connection to Redis failed.";
                break;
            case "RabbitMQ":
                return entry.Value.Status == HealthStatus.Healthy
                    ? "Rabbit is Ok"
                    : "Connection to Rabbit failed.";
                break;
            default:
                return entry.Value.Description;
                break;
        }
    }
}