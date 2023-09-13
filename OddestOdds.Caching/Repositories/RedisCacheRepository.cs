using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OddestOdds.Caching.Helper;
using OddestOdds.Caching.Services;
using OddestOdds.Common.Dto;
using StackExchange.Redis;

namespace OddestOdds.Caching.Repositories
{
    public class RedisCacheRepository : ICacheRepository
    {
        private readonly ILogger<RedisCacheRepository> _logger;
        private readonly IDatabase _database;

        public RedisCacheRepository(ILogger<RedisCacheRepository> logger, IRedisService redisService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _database = redisService.GetDatabase() ?? throw new ArgumentNullException(nameof(redisService));
        }

        public async Task CacheFixtureAsync(FixtureDto fixture)
        {
            try
            {
                var key = RedisKeyConstants.FixtureDetails(fixture.Id);
                ;
                var fixtureDetails = JsonConvert.SerializeObject(fixture);

                await _database.StringSetAsync(key, fixtureDetails);

                _logger.LogInformation("Successfully cached Fixture with ID: {FixtureId}", fixture.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache Fixture with ID: {FixtureId}", fixture.Id);
            }
        }

        public async Task CacheMarketAsync(MarketDto market)
        {
            try
            {
                var key = RedisKeyConstants.MarketDetails(market.Id);
                var marketDetails = JsonConvert.SerializeObject(market);

                await _database.StringSetAsync(key, marketDetails);
                _logger.LogInformation("Successfully cached Market with ID: {MarketId}", market.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache Market with ID: {MarketId}", market.Id);
            }
        }

        public async Task CacheMarketSelectionAsync(MarketSelectionDto selection)
        {
            try
            {
                var key = RedisKeyConstants.MarketSelectionDetails(selection.Id);
                var selectionDetails = JsonConvert.SerializeObject(selection);

                await _database.StringSetAsync(key, selectionDetails);

                _logger.LogInformation("Successfully cached MarketSelection with ID: {SelectionId}", selection.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache MarketSelection with ID: {SelectionId}", selection.Id);
            }
        }

        public async Task<FixtureDto?> GetCachedFixtureAsync(Guid fixtureId)
        {
            try
            {
                var key = RedisKeyConstants.FixtureDetails(fixtureId);
                var fixtureDetailsJson = await _database.StringGetAsync(key);
                if (fixtureDetailsJson.IsNullOrEmpty)
                {
                    _logger.LogWarning("Fixture with ID: {FixtureId} not found in cache", fixtureId);
                    return null;
                }

                var fixture = JsonConvert.DeserializeObject<FixtureDto>(fixtureDetailsJson!);

                return fixture;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch cached Fixture with ID: {FixtureId}", fixtureId);
                return null;
            }
        }

        public async Task<MarketDto?> GetCachedMarketAsync(Guid marketId)
        {
            try
            {
                var key = RedisKeyConstants.MarketDetails(marketId);
                var marketDetailsJson = await _database.StringGetAsync(key);
                if (marketDetailsJson.IsNullOrEmpty)
                {
                    _logger.LogWarning("Market with ID: {MarketId} not found in cache", marketId);
                    return null;
                }

                var market = JsonConvert.DeserializeObject<MarketDto>(marketDetailsJson!);

                return market;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch cached Market with ID: {MarketId}", marketId);
                return null;
            }
        }

        public async Task<MarketSelectionDto?> GetCachedMarketSelectionAsync(Guid selectionId)
        {
            try
            {
                var key = RedisKeyConstants.MarketSelectionDetails(selectionId);
                var selectionDetailsJson = await _database.StringGetAsync(key);
                if (selectionDetailsJson.IsNullOrEmpty)
                {
                    _logger.LogWarning("MarketSelection with ID: {SelectionId} not found in cache", selectionId);
                    return null;
                }

                return JsonConvert.DeserializeObject<MarketSelectionDto>(selectionDetailsJson!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch cached MarketSelection with ID: {SelectionId}", selectionId);
                return null;
            }
        }

        public async Task<IEnumerable<FixtureDto>> GetAllCachedFixturesAsync()
        {
            var fixtures = new List<FixtureDto>();
            try
            {
                var keys = new List<RedisKey>();
                var endpoints = _database.Multiplexer.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = _database.Multiplexer.GetServer(endpoint);
                    var dbKeys = server.Keys(pattern: "fixture:*");
                    keys.AddRange(dbKeys);
                }

                foreach (var key in keys)
                {
                    var fixtureDetailsJson = await _database.StringGetAsync(key);
                    if (!fixtureDetailsJson.IsNullOrEmpty)
                    {
                        var fixture = JsonConvert.DeserializeObject<FixtureDto>(fixtureDetailsJson);
                        if (fixture != null)
                        {
                            fixtures.Add(fixture);
                        }
                    }
                }

                _logger.LogInformation("Successfully fetched all cached fixtures");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all cached fixtures");
            }

            return fixtures;
        }

        public async Task InvalidateCacheAsync(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
                _logger.LogInformation("Successfully invalidated cache for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invalidate cache for key: {Key}", key);
            }
        }
    }
}