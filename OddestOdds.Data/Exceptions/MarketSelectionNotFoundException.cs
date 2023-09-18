namespace OddestOdds.Data.Exceptions;

public class MarketSelectionNotFoundException : Exception
{
    public MarketSelectionNotFoundException(string message, Guid selectionId) : base(message)
    {
        SelectionId = selectionId;
    }

    public Guid SelectionId { get; }
}