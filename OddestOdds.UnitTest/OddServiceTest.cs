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
    private readonly Mock<IFixtureRepository> _fixtureRepoMock;
    private readonly Mock<ICacheRepository> _cacheRepoMock;
    private readonly Mock<IValidator<CreateFixtureRequest>> _createFixtureRequestValidatorMock;
    private readonly Mock<IValidator<CreateOddRequest>> _createOddRequestValidatorMock;
    private readonly Mock<IValidator<PushOddsRequest>> _pushOddsValidatorMock;
    private readonly Mock<IMessagePublisherService> _messagePublisherMock;
    private readonly OddService _service;

    public OddServiceTests()
    {
        _fixtureRepoMock = new Mock<IFixtureRepository>();
        _cacheRepoMock = new Mock<ICacheRepository>();
        Mock<ILogger<OddService>> loggerMock = new();
        _createFixtureRequestValidatorMock = new Mock<IValidator<CreateFixtureRequest>>();
        _createOddRequestValidatorMock = new Mock<IValidator<CreateOddRequest>>();
        _messagePublisherMock = new Mock<IMessagePublisherService>();
        _pushOddsValidatorMock = new Mock<IValidator<PushOddsRequest>>();

        _service = new OddService(_fixtureRepoMock.Object,
            _cacheRepoMock.Object,
            loggerMock.Object,
            _createFixtureRequestValidatorMock.Object,
            _createOddRequestValidatorMock.Object,
            _pushOddsValidatorMock.Object,
            _messagePublisherMock.Object);
    }

    [Fact]
    public async Task CreateFixtureAsync_ShouldReturnFixtureCreatedResponse()
    {
        // Arrange

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


        _fixtureRepoMock.Setup(repo => repo.CreateFixtureAsync(It.IsAny<Fixture>()))
            .Callback<Fixture>(f => f.Id = fixture.Id);

        _createFixtureRequestValidatorMock.Setup(v => v.ValidateAsync(request, new CancellationToken()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var response = await _service.CreateFixtureAsync(request);

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
        var createOddRequest = new CreateOddRequest()
        {
            FixtureId = Guid.NewGuid(),
            SelectionName = "Test Selection",
            OddValue = 4,
            MarketId = Guid.NewGuid(),
            Side = MarketSelectionSide.Away
        };

        _createOddRequestValidatorMock.Setup(v => v.ValidateAsync(createOddRequest, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _cacheRepoMock.Setup(c => c.GetCachedMarketAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new MarketDto()
            {
                FixtureId = createOddRequest.FixtureId,
                Name = "Test Market",
                Id = createOddRequest.MarketId
            });
        _cacheRepoMock.Setup(c => c.GetCachedFixtureAsync(It.IsAny<Guid>())).ReturnsAsync(new FixtureDto()
        {
            AwayTeam = "A team",
            HomeTeam = "H team",
            FixtureName = "Test fixture",
            MarketIds = new List<Guid>() { createOddRequest.MarketId },
            Id = createOddRequest.FixtureId
        });

        // Act
        await _service.CreateOddAsync(createOddRequest);

        // Assert
        _messagePublisherMock.Verify(pub => pub.PublishMessageAsync(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOddAsync_ShouldHaveToDeleteInvalidateCacheAndPublishDeletedMessage()
    {
        // Arrange
        _fixtureRepoMock.Setup(f => f.DeleteMarketSelectionAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

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

        _cacheRepoMock.Setup(c => c.GetCachedMarketSelectionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(marketSelection.ToDto());
        _cacheRepoMock.Setup(c => c.GetCachedMarketAsync(It.IsAny<Guid>())).ReturnsAsync(marketDto);

        // Act
        await _service.DeleteOddAsync(marketSelection.Id);

        // Assert 
        _cacheRepoMock.Verify(c => c.CacheMarketAsync(It.Is<MarketDto>(m => m.SelectionIds.Count == 0)),
            Times.Once());
        _cacheRepoMock.Verify(c => c.InvalidateCacheAsync(RedisKeyConstants.MarketSelectionDetails(marketSelection.Id)),
            Times.Once);
        _messagePublisherMock.Verify(m => m.PublishMessageAsync(It.IsAny<OddDeletedMessage>()),
            Times.Once());
        _fixtureRepoMock.Verify(f => f.DeleteMarketSelectionAsync(marketSelection.Id), Times.Once);
    }

    [Fact]
    public async Task PushOddAsync_ShouldPublishMessages()
    {
        // Arrange
        var request = new PushOddsRequest
        {
            PushAll = false,
            MarketSelectionIds = new List<Guid> { Guid.NewGuid() }
        };

        var marketSelection = new MarketSelectionDto
        {
            Id = Guid.NewGuid(),
            MarketId = Guid.NewGuid(),
            OddValue = 1,
            Name = "Test Selection"
        };
        var market = new MarketDto
        {
            Id = marketSelection.MarketId,
            FixtureId = Guid.NewGuid(),
            SelectionIds = new List<Guid>()
            {
                marketSelection.Id
            },
            Name = "Test Market"
        };
        var fixture = new FixtureDto
        {
            Id = market.FixtureId,
            AwayTeam = "A team",
            MarketIds = new List<Guid>()
            {
                market.Id
            }
        };

        _pushOddsValidatorMock.Setup(v => v.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _cacheRepoMock.Setup(c => c.GetCachedMarketSelectionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(marketSelection);
        _cacheRepoMock.Setup(c => c.GetCachedMarketAsync(It.IsAny<Guid>()))
            .ReturnsAsync(market);
        _cacheRepoMock.Setup(c => c.GetCachedFixtureAsync(It.IsAny<Guid>()))
            .ReturnsAsync(fixture);

        // Act
        await _service.PushOddAsync(request);

        // Assert
        _messagePublisherMock.Verify(m => m.PublishMessageAsync(It.IsAny<PushedOddMessage>()), Times.AtLeastOnce);
    }
}