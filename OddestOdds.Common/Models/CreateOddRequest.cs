using OddestOdds.Data.Enums;

namespace OddestOdds.Common.Models;

public class CreateOddRequest
{
    public Guid FixtureId { get; set; }
    public Guid MarketId { get; set; }
    public string SelectionName { get; set; } = string.Empty;
    public decimal OddValue { get; set; }
    public MarketSelectionSide Side { get; set; }
}