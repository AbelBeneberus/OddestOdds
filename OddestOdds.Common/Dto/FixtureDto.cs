namespace OddestOdds.Common.Dto;

public class FixtureDto
{
    public Guid Id { get; set; }
    public string FixtureName { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public List<Guid> MarketIds { get; set; } = new List<Guid>();
}