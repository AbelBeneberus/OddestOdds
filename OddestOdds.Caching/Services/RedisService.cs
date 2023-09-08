using StackExchange.Redis;

namespace OddestOdds.Caching.Services;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _redisConnection;

    public RedisService(string connectionString)
    {
        _redisConnection = ConnectionMultiplexer.Connect(connectionString);
    }

    public IDatabase GetDatabase()
    {
        return _redisConnection.GetDatabase();
    }
}