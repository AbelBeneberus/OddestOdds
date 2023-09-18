using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OddestOdds.Data.Database;
using OddestOdds.Data.Exceptions;
using OddestOdds.Data.Models;

namespace OddestOdds.Data.Repository;

public class FixtureRepository : IFixtureRepository
{
    private readonly FixtureDbContext _dbContext;
    private readonly ILogger<FixtureRepository> _logger;


    public FixtureRepository(FixtureDbContext dbContext, ILogger<FixtureRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task CreateFixtureAsync(Fixture fixture)
    {
        await _dbContext.Fixtures.AddAsync(fixture);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateMarketSelectionAsync(MarketSelection selection)
    {
        await _dbContext.MarketSelections.AddAsync(selection);
        await _dbContext.SaveChangesAsync();
    }

    public Task UpdateMarketSelectionAsync(MarketSelection selection)
    {
        _dbContext.Entry(selection).State = EntityState.Modified;
        return _dbContext.SaveChangesAsync();
    }

    public Task<MarketSelection> GetMarketSelectionAsync(Guid marketSelectionId)
    {
        return _dbContext.MarketSelections.FirstOrDefaultAsync(ms => ms.Id == marketSelectionId)!;
    }

    public async Task DeleteMarketSelectionAsync(Guid marketSelectionId)
    {
        var marketSelection = await _dbContext.MarketSelections.FindAsync(marketSelectionId);
        if (marketSelection == null)
        {
            throw new MarketSelectionNotFoundException($"MarketSelection not found with id {marketSelectionId}",
                marketSelectionId);
        }

        _dbContext.MarketSelections.Remove(marketSelection);
        await _dbContext.SaveChangesAsync();
    }
}