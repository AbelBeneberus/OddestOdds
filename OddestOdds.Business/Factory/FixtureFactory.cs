using OddestOdds.Common.Models;
using OddestOdds.Data.Models;

namespace OddestOdds.Business.Factory;

public static class FixtureFactory
{
    public static Fixture CreateFrom(CreateFixtureRequest request)
    {
        var fixture = new Fixture
        {
            AwayTeam = request.AwayTeam,
            HomeTeam = request.HomeTeam,
            FixtureName = request.FixtureName,
            Markets = request.Markets.Select(marketRequest => new Market
            {
                MarketName = marketRequest.MarketName,
                Selections = marketRequest.Selections.Select(selectionRequest => new MarketSelection
                {
                    Name = selectionRequest.Name,
                    Odd = selectionRequest.OddValue,
                    Side = selectionRequest.SelectionSide
                }).ToList()
            }).ToList()
        };
        return fixture;
    }
}