using System.Text;

namespace OddestOdds.Common.Messages;

public class PushedOddMessage : Message
{
    public Guid Id { get; set; }
    public Guid FixtureId { get; set; }
    public Guid MarketId { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = String.Empty;
    public decimal Odd { get; set; }
    public string SelectionName { get; set; } = string.Empty;
    public string MarketName { get; set; } = string.Empty;
    public string FixtureName { get; set; } = string.Empty;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Incoming Odd for FixtureId: {FixtureId}");
        sb.AppendLine($"  └── FixtureName: {FixtureName}");
        sb.AppendLine($"  └── HomeTeam: {HomeTeam}");
        sb.AppendLine($"  └── AwayTeam: {AwayTeam}");
        sb.AppendLine($"  └── MarketId: {MarketId}");
        sb.AppendLine($"  └── MarketName: {MarketName}");
        sb.AppendLine($"  └── Market Selection Id: {Id}");
        sb.AppendLine($"  └── Market Selection Name: {SelectionName}");
        sb.AppendLine($"  └── Odd: {Odd}");

        return sb.ToString();
    }
}