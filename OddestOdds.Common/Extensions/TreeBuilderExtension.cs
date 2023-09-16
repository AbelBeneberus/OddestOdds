using System.Text;
using OddestOdds.Common.Models;

namespace OddestOdds.Common.Extensions;

public static class TreeBuilderExtension
{
    public static string ToTreeStructure(this GetOddResponse response)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Total Fixtures Count: {response.TotalFixturesCount}");
        sb.AppendLine($"Total Markets Count: {response.TotalMarketsCount}");
        sb.AppendLine($"Total Market Selections Count: {response.TotalMarketSelectionsCount}");

        foreach (var fixture in response.Fixtures)
        {
            sb.AppendLine($"└── FixtureId: {fixture.FixtureId}");
            sb.AppendLine($"    └── FixtureName: {fixture.FixtureName}");
            sb.AppendLine($"        └── HomeTeam: {fixture.HomeTeam}");
            sb.AppendLine($"        └── AwayTeam: {fixture.AwayTeam}");
            sb.AppendLine($"        └── MarketsCount: {fixture.MarketsCount}");
            sb.AppendLine($"        └── MarketSelectionsCount: {fixture.MarketSelectionsCount}");

            var markets = response.Markets.Where(m => m.FixtureId == fixture.FixtureId);
            foreach (var market in markets)
            {
                sb.AppendLine($"            └── MarketId: {market.MarketId}");
                sb.AppendLine($"                └── Name: {market.Name}");
                sb.AppendLine($"                └── SelectionCount: {market.SelectionCount}");

                var selections = response.Selections.Where(s => s.MarketId == market.MarketId);
                foreach (var selection in selections)
                {
                    sb.AppendLine($"                    └── SelectionId: {selection.SelectionId}");
                    sb.AppendLine($"                        ├── Name: {selection.Name}");
                    sb.AppendLine($"                        ├── OddValue: {selection.OddValue}");
                    sb.AppendLine($"                        └── SelectionSide: {selection.SelectionSide}");
                }
            }
        }

        return sb.ToString();
    }
}