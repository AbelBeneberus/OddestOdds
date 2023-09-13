using OddestOdds.Data.Enums;

namespace OddestOdds.Common.Models;

public class GetOddResponse
{
    public int TotalFixturesCount { get; set; }
    public int TotalMarketsCount { get; set; }
    public int TotalMarketSelectionsCount { get; set; }
    public IEnumerable<FixtureResponse> Fixtures { get; set; } = new List<FixtureResponse>();
    public IEnumerable<MarketResponse> Markets { get; set; } = new List<MarketResponse>();
    public IEnumerable<MarketSelectionResponse> Selections { get; set; } = new List<MarketSelectionResponse>();

    public class FixtureResponse
    {
        public Guid FixtureId { get; set; }
        public string FixtureName { get; set; } = string.Empty;
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public int MarketsCount { get; set; }
        public int MarketSelectionsCount { get; set; }
    }

    public class MarketResponse
    {
        public Guid MarketId { get; set; }
        public Guid FixtureId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SelectionCount { get; set; }
    }

    public class MarketSelectionResponse
    {
        public Guid SelectionId { get; set; }
        public Guid MarketId { get; set; }
        public Guid FixtureId { get; set; }
        public string Name { get; set; } = string.Empty;
        public MarketSelectionSide SelectionSide { get; set; }
        public decimal OddValue { get; set; }
    }
}