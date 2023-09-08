using OddestOdds.Data.Enums;

namespace OddestOdds.Data.Models;

public class MarketSelection : BaseEntity
{
    public Guid MarketId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Odd { get; set; }
    public MarketSelectionSide Side { get; set; }
}