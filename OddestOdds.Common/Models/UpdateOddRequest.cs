namespace OddestOdds.Common.Models;

public class UpdateOddRequest
{
    public Guid MarketSelectionId { get; set; }
    public decimal NewOddValue { get; set; }
}