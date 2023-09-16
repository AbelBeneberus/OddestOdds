namespace OddestOdds.Common.Messages;

public class OddUpdatedMessage : Message
{
    public Guid MarketSelectionId { get; set; }
    public decimal NewOddValue { get; set; }

    public override string ToString()
    {
        return $"Odd Updated for {MarketSelectionId} : New Odd Value : {NewOddValue}";
    }
}