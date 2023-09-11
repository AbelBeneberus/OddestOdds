using Microsoft.Extensions.Logging;
using Moq;
using OddestOdds.Caching.Repositories;
using OddestOdds.Caching.Services;
using OddestOdds.Common.Dto;
using StackExchange.Redis;
using Xunit;

namespace OddestOdds.UnitTest;

public class RedisCacheRepositoryTests
{
    [Fact]
    public async Task CacheFixtureAsync_ShouldCacheSuccessfully()
    {
        var mockLogger = new Mock<ILogger<RedisCacheRepository>>();
        var mockRedisService = new Mock<IRedisService>();
        var mockDatabase = new Mock<IDatabase>();

        mockRedisService.Setup(m => m.GetDatabase()).Returns(mockDatabase.Object);

        var repo = new RedisCacheRepository(mockLogger.Object, mockRedisService.Object);

        var fixture = new FixtureDto { Id = Guid.NewGuid(), FixtureName = "Test Fixture" };

        await repo.CacheFixtureAsync(fixture);

        mockDatabase.Verify(
            db => db.StringSetAsync(It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                null,
                false,
                When.Always,
                CommandFlags.None),
            Times.Once);
    }
}