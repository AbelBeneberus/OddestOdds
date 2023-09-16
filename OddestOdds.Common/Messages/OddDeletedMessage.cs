namespace OddestOdds.Common.Messages;

public class OddDeletedMessage : Message
{
    public Guid MarketSelectionId { get; set; }

    public override string ToString()
    {
        return $"Odd deleted for marketSelectionId: {MarketSelectionId}";
    }
}