using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OddestOdds.Data.Database;
using OddestOdds.Data.Models;
using OddestOdds.HandlerApp;


namespace OddestOdds.IntegrationTest.Drivers;

public class HandlerDriver
{
    private readonly TestServer _server;
    private readonly HttpClient _client;
    private readonly FixtureDbContext _context;

    public HandlerDriver()
    {
        _server = new TestServer(new WebHostBuilder()
            .UseEnvironment("Testing")
            .ConfigureAppConfiguration((context, config) => { config.AddJsonFile("appsettings.Test.json"); })
            .UseStartup<Startup>()
        );
        _context = _server.Host.Services.GetRequiredService<FixtureDbContext>();
        EnsureDatabaseMigrated();
        _client = _server.CreateClient();
    }

    public HttpClient GetClient()
    {
        return _client;
    }

    public TestServer GetServer()
    {
        return _server;
    }

    public void Dispose()
    {
        _client.Dispose();
        _server.Dispose();
    }

    public async Task<MarketSelection> GetCreatedOddAsync(string oddName)
    {
        await ReloadTrackedEntities();
        var odd = await _context.MarketSelections.FirstOrDefaultAsync(selection => selection.Name == oddName);
        return odd;
    }

    public async Task<Guid> GetOddId()
    {
        var selection = await _context.MarketSelections.FirstAsync();
        return selection.Id;
    }

    private void EnsureDatabaseMigrated()
    {
        if (!_context.Database.CanConnect())
        {
            _context.Database.Migrate();
        }
    }

    private async Task ReloadTrackedEntities()
    {
        foreach (var entity in _context.ChangeTracker.Entries().ToList())
        {
            await entity.ReloadAsync();
        }
    }
}