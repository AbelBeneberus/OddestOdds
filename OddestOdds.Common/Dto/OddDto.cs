namespace OddestOdds.Common.Dto;

public class OddDto
{
    public string MarketName { get; set; } = string.Empty;
    public string FixtureName { get; set; } = string.Empty;
    public Guid MarketSelectionId { get; set; }
    public Guid MarketId { get; set; }
    public Guid FixtureId { get; set; }
    public decimal Odd { get; set; }
    public string TeamSelectionSide { get; set; } = string.Empty;
}