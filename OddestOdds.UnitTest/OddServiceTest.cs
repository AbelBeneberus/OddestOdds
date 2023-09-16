using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OddestOdds.Business.Services;
using OddestOdds.Caching.Helper;
using OddestOdds.Caching.Repositories;
using OddestOdds.Common.Dto;
using OddestOdds.Common.Extensions;
using OddestOdds.Common.Messages;
using OddestOdds.Common.Models;
using OddestOdds.Data.Enums;
using OddestOdds.Data.Models;
using OddestOdds.Data.Repository;
using OddestOdds.Messaging.Services;
using Xunit;

namespace OddestOdds.UnitTest;

public class OddServiceTests
{
    [Fact]
    public async Task CreateFixtureAsync_ShouldReturnFixtureCreatedResponse()
    {
        // Arrange
        var fixtureRepoMock = new Mock<IFixtureRepository>();
        var cacheRepoMock = new Mock<ICacheRepository>();
        var loggerMock = new Mock<ILogger<OddService>>();
        var createFixtureRequestValidatorMock = new Mock<IValidator<CreateFixtureRequest>>();
        var createOddRequestValidatorMock = new Mock<IValidator<CreateOddRequest>>();
        var messagePublisherMock = new Mock<IMessagePublisherService>();

        var service = new OddService(fixtureRepoMock.Object,
            cacheRepoMock.Object,
            loggerMock.Object,
            createFixtureRequestValidatorMock.Object,
            createOddRequestValidatorMock.Object,
            messagePublisherMock.Object);

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

        // Act
        var response = await service.CreateFixtureAsync(request);

        // Assert
        response.Should().BeOfType<FixtureCreatedResponse>();
        response.FixtureId.Should().Be(fixture.Id);
        response.MarketIds.Length.Should().Be(1);
        response.MarketSelectionIds.Length.Should().Be(2);
    }

    [Fact]
    public async Task CreateOddAsync_ShouldPublishMessage()
    {
        // Arrange
        var fixtureRepoMock = new Mock<IFixtureRepository>();
        var cacheRepoMock = new Mock<ICacheRepository>();
        var loggerMock = new Mock<ILogger<OddService>>();
        var createFixtureRequestValidatorMock = new Mock<IValidator<CreateFixtureRequest>>();
        var createOddRequestValidatorMock = new Mock<IValidator<CreateOddRequest>>();
        var messagePublisherMock = new Mock<IMessagePublisherService>();

        var service = new OddService(fixtureRepoMock.Object,
            cacheRepoMock.Object,
            loggerMock.Object,
            createFixtureRequestValidatorMock.Object,
            createOddRequestValidatorMock.Object,
            messagePublisherMock.Object);

        var createOddRequest = new CreateOddRequest()
        {
            FixtureId = Guid.NewGuid(),
            SelectionName = "Test Selection",
            OddValue = 4,
            MarketId = Guid.NewGuid(),
            Side = MarketSelectionSide.Away
        };

        createOddRequestValidatorMock.Setup(v => v.ValidateAsync(createOddRequest, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        cacheRepoMock.Setup(c => c.GetCachedMarketAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new MarketDto()
            {
                FixtureId = createOddRequest.FixtureId,
                Name = "Test Market",
                Id = createOddRequest.MarketId
            });
        cacheRepoMock.Setup(c => c.GetCachedFixtureAsync(It.IsAny<Guid>())).ReturnsAsync(new FixtureDto()
        {
            AwayTeam = "A team",
            HomeTeam = "H team",
            FixtureName = "Test fixture",
            MarketIds = new List<Guid>() { createOddRequest.MarketId },
            Id = createOddRequest.FixtureId
        });

        // Act
        await service.CreateOddAsync(createOddRequest);

        // Assert
        messagePublisherMock.Verify(pub => pub.PublishMessageAsync(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOddAsync_ShouldHaveToDeleteInvalidateCacheAndPublishDeletedMessage()
    {
        // Arrange
        var fixtureRepoMock = new Mock<IFixtureRepository>();
        var cacheRepoMock = new Mock<ICacheRepository>();
        var loggerMock = new Mock<ILogger<OddService>>();
        var createFixtureRequestValidatorMock = new Mock<IValidator<CreateFixtureRequest>>();
        var createOddRequestValidatorMock = new Mock<IValidator<CreateOddRequest>>();
        var messagePublisherMock = new Mock<IMessagePublisherService>();

        var service = new OddService(fixtureRepoMock.Object,
            cacheRepoMock.Object,
            loggerMock.Object,
            createFixtureRequestValidatorMock.Object,
            createOddRequestValidatorMock.Object,
            messagePublisherMock.Object);

        fixtureRepoMock.Setup(f => f.DeleteMarketSelectionAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var marketSelection = new MarketSelection()
        {
            MarketId = Guid.NewGuid(),
            Side = MarketSelectionSide.Away,
            Name = "Test X",
            Id = Guid.NewGuid(),
            Odd = 1
        };

        var marketDto = new MarketDto()
        {
            FixtureId = Guid.NewGuid(),
            Id = marketSelection.MarketId,
            Name = "Test Markets",
            SelectionIds = new List<Guid>()
            {
                marketSelection.Id
            }
        };

        cacheRepoMock.Setup(c => c.GetCachedMarketSelectionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(marketSelection.ToDto());
        cacheRepoMock.Setup(c => c.GetCachedMarketAsync(It.IsAny<Guid>())).ReturnsAsync(marketDto);

        // Act
        await service.DeleteOddAsync(marketSelection.Id);

        // Assert 
        cacheRepoMock.Verify(c => c.CacheMarketAsync(It.Is<MarketDto>(m => m.SelectionIds.Count == 0)),
            Times.Once());
        cacheRepoMock.Verify(c => c.InvalidateCacheAsync(RedisKeyConstants.MarketSelectionDetails(marketSelection.Id)),
            Times.Once);
        messagePublisherMock.Verify(m => m.PublishMessageAsync(It.IsAny<OddDeletedMessage>()),
            Times.Once());
        fixtureRepoMock.Verify(f => f.DeleteMarketSelectionAsync(marketSelection.Id), Times.Once);
    }
}