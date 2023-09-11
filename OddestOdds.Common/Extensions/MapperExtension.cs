using OddestOdds.Common.Dto;
using OddestOdds.Data.Models;

namespace OddestOdds.Common.Extensions;

public static class MapperExtension
{
    public static FixtureDto ToDto(this Fixture fixture)
    {
        return new FixtureDto()
        {
            Id = fixture.Id,
            AwayTeam = fixture.AwayTeam,
            HomeTeam = fixture.HomeTeam,
            FixtureName = fixture.FixtureName,
            MarketIds = fixture.Markets.Select(m => m.Id).ToList()
        };
    }

    public static MarketDto ToDto(this Market market)
    {
        return new MarketDto()
        {
            Id = market.Id,
            Name = market.MarketName,
            SelectionIds = market.Selections.Select(s => s.Id).ToList()
        };
    }

    public static MarketSelectionDto ToDto(this MarketSelection selection)
    {
        return new MarketSelectionDto()
        {
            Id = selection.Id,
            Name = selection.Name,
            Side = selection.Side,
            OddValue = selection.Odd,
            MarketId = selection.MarketId
        };
    }
}