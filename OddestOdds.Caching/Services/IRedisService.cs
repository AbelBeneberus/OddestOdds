using StackExchange.Redis;

namespace OddestOdds.Caching.Services
{
    public interface IRedisService
    {
        IDatabase GetDatabase();
    }
}