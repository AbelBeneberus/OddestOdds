using OddestOdds.Common.Dto;

namespace OddestOdds.Caching.Repositories;

public interface ICacheRepository
{
    Task CacheFixtureAsync(FixtureDto fixture);
    Task CacheMarketAsync(MarketDto market);
    Task CacheMarketSelectionAsync(MarketSelectionDto selection);
    Task<FixtureDto?> GetCachedFixtureAsync(Guid fixtureId);
    Task<MarketDto?> GetCachedMarketAsync(Guid marketId);
    Task<MarketSelectionDto?> GetCachedMarketSelectionAsync(Guid selectionId);
    Task InvalidateCacheAsync(string key);
}