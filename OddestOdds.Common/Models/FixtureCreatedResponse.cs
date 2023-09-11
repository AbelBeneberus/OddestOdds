namespace OddestOdds.Common.Models;

public class FixtureCreatedResponse
{
    public Guid FixtureId { get; set; }
    public Guid[] MarketIds { get; set; } = Array.Empty<Guid>();
    public Guid[] MarketSelectionIds { get; set; } = Array.Empty<Guid>();
}