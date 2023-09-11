namespace OddestOdds.Common.Dto;

public class MarketDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Guid> SelectionIds { get; set; } = new List<Guid>();
}