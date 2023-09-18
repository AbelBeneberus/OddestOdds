namespace OddestOdds.Common.Models;

public class PushOddsRequest
{
    public bool PushAll { get; set; } = false;
    public IEnumerable<Guid> MarketSelectionIds { get; set; } = new List<Guid>();
}