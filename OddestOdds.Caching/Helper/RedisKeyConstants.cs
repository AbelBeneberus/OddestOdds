namespace OddestOdds.Caching.Helper;

public static class RedisKeyConstants
{
    public static string FixtureDetails(Guid id) => $"fixture:{id}";
    public static string MarketDetails(Guid id) => $"market:{id}";
    public static string MarketSelectionDetails(Guid id) => $"selection:{id}";
}