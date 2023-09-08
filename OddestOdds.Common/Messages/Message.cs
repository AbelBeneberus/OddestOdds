namespace OddestOdds.Common.Messages;

public class Message
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
}