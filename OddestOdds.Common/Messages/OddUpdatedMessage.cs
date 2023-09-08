using System.Text;

namespace OddestOdds.Common.Messages;

public class OddUpdatedMessage : Message
{
    public Guid MarketSelectionId { get; set; }
    public Guid MarketId { get; set; }
    public Guid FixtureId { get; set; }
    public string SelectionName { get; set; } = string.Empty;
    public string MarketName { get; set; } = string.Empty;
    public string FixtureName { get; set; } = string.Empty;
    public decimal Odd { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Odd Updated for FixtureId: {FixtureId}");
        sb.AppendLine($"  └── FixtureName: {FixtureName}");
        sb.AppendLine($"  └── MarketName: {MarketName}");
        sb.AppendLine($"  └── MarketId: {MarketId}");
        sb.AppendLine($"  └── Market Selection Name: {SelectionName}");
        sb.AppendLine($"  └── Market Selection Id: {MarketSelectionId}");
        sb.AppendLine($"  └── Odd: {Odd}");

        return sb.ToString();
    }
}