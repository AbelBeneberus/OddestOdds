using OddestOdds.Data.Enums;

namespace OddestOdds.Common.Models;

public class CreateFixtureRequest
{
    public string FixtureName { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;

    public IEnumerable<MarketRequest> Markets { get; set; } = new List<MarketRequest>();

    public class MarketRequest
    {
        public string MarketName { get; set; } = string.Empty;
        public IEnumerable<MarketSelectionRequest> Selections { get; set; } = new List<MarketSelectionRequest>();

        public class MarketSelectionRequest
        {
            public string Name { get; set; } = string.Empty;
            public decimal OddValue { get; set; }
            public MarketSelectionSide SelectionSide { get; set; }
        }
    }
}