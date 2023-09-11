using OddestOdds.Data.Enums;

namespace OddestOdds.Common.Dto;

public class MarketSelectionDto
{
    public Guid Id { get; set; }
    public Guid MarketId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal OddValue { get; set; }
    public MarketSelectionSide Side { get; set; }
}