namespace OddestOdds.Data.Models;

public class Fixture : BaseEntity
{                                                                              
    public string FixtureName { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public IEnumerable<Market> Markets { get; set; } = new List<Market>();
}