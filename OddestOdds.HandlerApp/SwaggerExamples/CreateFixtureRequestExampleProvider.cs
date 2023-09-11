using OddestOdds.Common.Models;
using OddestOdds.Data.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace OddestOdds.HandlerApp.SwaggerExamples;

public class CreateFixtureRequestExampleProvider : IExamplesProvider<CreateFixtureRequest>
{
    public CreateFixtureRequest GetExamples()
    {
        return new CreateFixtureRequest()
        {
            AwayTeam = "Manchester",
            HomeTeam = "Arsenal",
            FixtureName = "Premier League",
            Markets = new List<CreateFixtureRequest.MarketRequest>()
            {
                new CreateFixtureRequest.MarketRequest()
                {
                    MarketName = "Double Chance",
                    Selections = new List<CreateFixtureRequest.MarketRequest.MarketSelectionRequest>()
                    {
                        new CreateFixtureRequest.MarketRequest.MarketSelectionRequest()
                        {
                            Name = "1",
                            OddValue = 10,
                            SelectionSide = MarketSelectionSide.Home
                        },
                        new CreateFixtureRequest.MarketRequest.MarketSelectionRequest()
                        {
                            Name = "X",
                            OddValue = 2,
                            SelectionSide = MarketSelectionSide.Draw
                        },
                        new CreateFixtureRequest.MarketRequest.MarketSelectionRequest()
                        {
                            Name = "2",
                            OddValue = 3,
                            SelectionSide = MarketSelectionSide.Away
                        }
                    }
                },
                new CreateFixtureRequest.MarketRequest()
                {
                    MarketName = "First Half",
                    Selections = new List<CreateFixtureRequest.MarketRequest.MarketSelectionRequest>()
                    {
                        new CreateFixtureRequest.MarketRequest.MarketSelectionRequest()
                        {
                            Name = "Arsenal",
                            OddValue = 2,
                            SelectionSide = MarketSelectionSide.Away
                        },
                        new CreateFixtureRequest.MarketRequest.MarketSelectionRequest()
                        {
                            Name = "X",
                            OddValue = 2,
                            SelectionSide = MarketSelectionSide.Draw
                        },
                        new CreateFixtureRequest.MarketRequest.MarketSelectionRequest()
                        {
                            Name = "Manchester",
                            OddValue = 3,
                            SelectionSide = MarketSelectionSide.Home
                        }
                    }
                }
            }
        };
    }
}