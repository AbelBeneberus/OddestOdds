using OddestOdds.Data.Models;

namespace OddestOdds.Data.Repository;

public interface IFixtureRepository
{
    Task CreateFixtureAsync(Fixture fixture);
    Task CreateMarketSelectionAsync(MarketSelection selection);
    Task UpdateMarketSelectionAsync(MarketSelection selection);
    Task<MarketSelection> GetMarketSelectionAsync(Guid marketSelectionId);
}