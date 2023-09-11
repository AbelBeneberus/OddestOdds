using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OddestOdds.Business.Services;
using OddestOdds.Caching.Repositories;
using OddestOdds.Common.Models;
using OddestOdds.Data.Enums;
using OddestOdds.Data.Models;
using OddestOdds.Data.Repository;
using Xunit;

namespace OddestOdds.UnitTest;

public class OddServiceTests
{
    [Fact]
    public async Task CreateFixtureAsync_ShouldReturnFixtureCreatedResponse()
    {
        var fixtureRepoMock = new Mock<IFixtureRepository>();
        var cacheRepoMock = new Mock<ICacheRepository>();
        var loggerMock = new Mock<ILogger<OddService>>();
        var createFixtureRequestValidatorMock = new Mock<IValidator<CreateFixtureRequest>>();
        var createOddRequestValidatorMock = new Mock<IValidator<CreateOddRequest>>();

        var service = new OddService(fixtureRepoMock.Object,
            cacheRepoMock.Object,
            loggerMock.Object,
            createFixtureRequestValidatorMock.Object,
            createOddRequestValidatorMock.Object);

        var request = new CreateFixtureRequest
        {
            FixtureName = "TestFixture",
            HomeTeam = "TeamA",
            AwayTeam = "TeamB",
            Markets = new List<CreateFixtureRequest.MarketRequest>()
            {
                new()
                {
                    MarketName = "m1",
                    Selections = new List<CreateFixtureRequest.MarketRequest.MarketSelectionRequest>()
                    {
                        new CreateFixtureRequest.MarketRequest.MarketSelectionRequest()
                        {
                            Name = "ms1",
                            OddValue = 1,
                            SelectionSide = MarketSelectionSide.Home
                        },
                        new CreateFixtureRequest.MarketRequest.MarketSelectionRequest()
                        {
                            Name = "ms2",
                            OddValue = 1,
                            SelectionSide = MarketSelectionSide.Draw
                        }
                    }
                }
            }
        };

        var fixture = new Fixture
        {
            Id = Guid.NewGuid(),
            FixtureName = request.FixtureName,
            HomeTeam = request.HomeTeam,
            AwayTeam = request.AwayTeam
        };


        fixtureRepoMock.Setup(repo => repo.CreateFixtureAsync(It.IsAny<Fixture>()))
            .Callback<Fixture>(f => f.Id = fixture.Id);

        createFixtureRequestValidatorMock.Setup(v => v.ValidateAsync(request, new CancellationToken()))
            .ReturnsAsync(new ValidationResult());

        var response = await service.CreateFixtureAsync(request);

        response.Should().BeOfType<FixtureCreatedResponse>();
        response.FixtureId.Should().Be(fixture.Id);
        response.MarketIds.Length.Should().Be(1);
        response.MarketSelectionIds.Length.Should().Be(2);
    }
}