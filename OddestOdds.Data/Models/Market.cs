namespace OddestOdds.Data.Models;

public class Market : BaseEntity
{
    public Guid FixtureId { get; set; }
    public string MarketName { get; set; } = string.Empty;
    public IEnumerable<MarketSelection> Selections { get; set; } = new List<MarketSelection>();
}