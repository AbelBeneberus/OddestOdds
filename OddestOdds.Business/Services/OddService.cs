using FluentValidation;
using Microsoft.Extensions.Logging;
using OddestOdds.Business.Factory;
using OddestOdds.Caching.Repositories;
using OddestOdds.Common.Dto;
using OddestOdds.Common.Exceptions;
using OddestOdds.Common.Extensions;
using OddestOdds.Common.Models;
using OddestOdds.Data.Models;
using OddestOdds.Data.Repository;

namespace OddestOdds.Business.Services;

public class OddService : IOddService
{
    private readonly IFixtureRepository _fixtureRepository;
    private readonly ICacheRepository _cacheRepository;
    private readonly ILogger<OddService> _logger;
    private readonly IValidator<CreateFixtureRequest> _createFixtureRequestValidator;
    private readonly IValidator<CreateOddRequest> _createOddRequestValidator;

    public OddService(
        IFixtureRepository fixtureRepository,
        ICacheRepository cacheRepository,
        ILogger<OddService> logger,
        IValidator<CreateFixtureRequest> createFixtureRequestValidator,
        IValidator<CreateOddRequest> createOddRequestValidator)
    {
        _fixtureRepository = fixtureRepository ?? throw new ArgumentNullException(nameof(fixtureRepository));
        _cacheRepository = cacheRepository ?? throw new ArgumentNullException(nameof(cacheRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _createFixtureRequestValidator = createFixtureRequestValidator ??
                                         throw new ArgumentNullException(nameof(createFixtureRequestValidator));
        _createOddRequestValidator = createOddRequestValidator ??
                                     throw new ArgumentNullException(nameof(createOddRequestValidator));
    }

    public async Task<FixtureCreatedResponse> CreateFixtureAsync(CreateFixtureRequest request)
    {
        await ValidateRequest(request);
        try
        {
            var fixture = CreateFixture(request);

            await SaveFixture(fixture);

            return await CacheAndRespond(fixture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new fixture");
            throw;
        }
    }

    public async Task CreateOddAsync(CreateOddRequest request)
    {
        _logger.LogInformation("Starting transaction for creating new market selection", request);

        var validationResult = await _createOddRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            _logger.LogError(
                "Validation failed for create odd request",
                new { Errors = validationResult.Errors, Request = request });

            throw new ValidationException("Validation failed", validationResult.Errors);
        }

        try
        {
            var marketSelection = new MarketSelection()
            {
                MarketId = request.MarketId,
                Name = request.SelectionName,
                Odd = request.OddValue,
                Side = request.Side
            };

            await _fixtureRepository.CreateMarketSelectionAsync(marketSelection);


            var cachedMarket = await _cacheRepository.GetCachedMarketAsync(request.MarketId);
            if (cachedMarket != null)
            {
                cachedMarket.SelectionIds.Add(marketSelection.Id);
                await _cacheRepository.CacheMarketAsync(cachedMarket);
            }

            await _cacheRepository.CacheMarketSelectionAsync(marketSelection.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while creating odd");
            throw;
        }
    }

    public async Task UpdateOddAsync(UpdateOddRequest request)
    {
        try
        {
            var selection = await _fixtureRepository.GetMarketSelectionAsync(request.MarketSelectionId);
            if (selection == null)
            {
                throw new MarketSelectionNotFoundException("Market selection not found", request.MarketSelectionId);
            }

            selection.Odd = request.NewOddValue;
            await _fixtureRepository.UpdateMarketSelectionAsync(selection);
            await _cacheRepository.CacheMarketSelectionAsync(selection.ToDto());
        }
        catch (MarketSelectionNotFoundException exception)
        {
            _logger.LogWarning(exception, "Market selection with Id : {RequestMarketSelectionId} not found",
                request.MarketSelectionId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An Error occured while updating market selection");
            throw;
        }
    }

    public async Task<GetOddResponse> GetAllOddsAsync()
    {
        var fixtures = await _cacheRepository.GetAllCachedFixturesAsync();
        return await GenerateGetOddResponse(fixtures);
    }

    public async Task<GetOddResponse> GetOddsByFixtureIds(IEnumerable<Guid> fixtureIds)
    {
        var fetchFixtureTasks = fixtureIds.Select(fixtureId => _cacheRepository.GetCachedFixtureAsync(fixtureId));
        var fixtures = await Task.WhenAll(fetchFixtureTasks);
        return await GenerateGetOddResponse(fixtures);
    }

    private async Task ValidateRequest(CreateFixtureRequest request)
    {
        var validationResult = await _createFixtureRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Validation failed for create fixture request",
                new { Errors = validationResult.Errors, Request = request });
            throw new ValidationException("Validation failed", validationResult.Errors);
        }
    }

    private Fixture CreateFixture(CreateFixtureRequest request)
    {
        return FixtureFactory.CreateFrom(request);
    }

    private async Task SaveFixture(Fixture fixture)
    {
        await _fixtureRepository.CreateFixtureAsync(fixture);
    }

    private async Task<FixtureCreatedResponse> CacheAndRespond(Fixture fixture)
    {
        _logger.LogInformation("Caching new fixture, markets, and market selections", fixture);

        var cacheFixtureTask = _cacheRepository.CacheFixtureAsync(fixture.ToDto());
        var cacheMarketTasks = fixture.Markets.Select(m => _cacheRepository.CacheMarketAsync(m.ToDto()));
        var cacheMarketSelectionTasks = fixture.Markets
            .SelectMany(ms => ms.Selections)
            .Select(ms => _cacheRepository.CacheMarketSelectionAsync(ms.ToDto()));

        await Task.WhenAll(cacheFixtureTask,
            Task.WhenAll(cacheMarketTasks),
            Task.WhenAll(cacheMarketSelectionTasks));

        return new FixtureCreatedResponse
        {
            FixtureId = fixture.Id,
            MarketIds = fixture.Markets.Select(m => m.Id).ToArray(),
            MarketSelectionIds = fixture.Markets.SelectMany(ms => ms.Selections).Select(s => s.Id).ToArray()
        };
    }

    private async Task<GetOddResponse> GenerateGetOddResponse(IEnumerable<FixtureDto> fixtures)
    {
        // Convert fixtures to a dictionary for faster lookup
        var fixtureDict = fixtures.ToDictionary(f => f.Id, f => f);

        // Fetch and map markets
        var markets = await FetchAndMapMarkets(fixtureDict);

        // Fetch and map selections
        var selections = await FetchAndMapSelections(markets);

        var fixtureResponse = MapFixturesToResponse(fixtureDict, markets).ToList();
        var marketResponse = MapMarketsToResponse(markets).ToList();
        var selectionResponse = MapSelectionsToResponse(selections, fixtureDict).ToList();

        return new GetOddResponse
        {
            Fixtures = fixtureResponse,
            Markets = marketResponse,
            Selections = selectionResponse,
            TotalFixturesCount = fixtureResponse.Count(),
            TotalMarketsCount = marketResponse.Count(),
            TotalMarketSelectionsCount = selectionResponse.Count()
        };
    }

    private async Task<Dictionary<Guid, MarketDto>> FetchAndMapMarkets(Dictionary<Guid, FixtureDto> fixtureDict)
    {
        var marketTasks = fixtureDict.Values
            .SelectMany(f => f.MarketIds).Distinct()
            .Select(m => _cacheRepository.GetCachedMarketAsync(m));

        var marketList = await Task.WhenAll(marketTasks);

        return marketList.ToDictionary(m => m.Id, m => m);
    }

    private async Task<Dictionary<Guid, MarketSelectionDto>> FetchAndMapSelections(
        Dictionary<Guid, MarketDto> marketDict)
    {
        var selectionTasks = marketDict.Values.SelectMany(m => m.SelectionIds).Distinct()
            .Select(s => _cacheRepository.GetCachedMarketSelectionAsync(s));
        var selectionList = await Task.WhenAll(selectionTasks);
        return selectionList.ToDictionary(s => s.Id, s => s);
    }

    private IEnumerable<GetOddResponse.FixtureResponse> MapFixturesToResponse(
        Dictionary<Guid, FixtureDto> fixtureDict,
        Dictionary<Guid, MarketDto> markets)
    {
        return fixtureDict.Values.Select(f =>
        {
            int totalSelections = f.MarketIds
                .Where(markets.ContainsKey)
                .Sum(marketId => markets[marketId].SelectionIds.Count);

            return new GetOddResponse.FixtureResponse
            {
                FixtureId = f.Id,
                FixtureName = f.FixtureName,
                AwayTeam = f.AwayTeam,
                HomeTeam = f.HomeTeam,
                MarketsCount = f.MarketIds.Count,
                MarketSelectionsCount = totalSelections
            };
        });
    }

    private IEnumerable<GetOddResponse.MarketResponse> MapMarketsToResponse(Dictionary<Guid, MarketDto> marketDict)
    {
        return marketDict.Values.Select(m => new GetOddResponse.MarketResponse
        {
            FixtureId = m.FixtureId,
            MarketId = m.Id,
            Name = m.Name,
            SelectionCount = m.SelectionIds.Count
        });
    }

    private IEnumerable<GetOddResponse.MarketSelectionResponse> MapSelectionsToResponse(
        Dictionary<Guid, MarketSelectionDto> selectionDict,
        Dictionary<Guid, FixtureDto> fixtureDict)
    {
        return selectionDict.Values.Select(s =>
        {
            var fixture = fixtureDict.Values.FirstOrDefault(f => f.MarketIds.Contains(s.MarketId));

            return new GetOddResponse.MarketSelectionResponse
            {
                FixtureId = fixture?.Id ?? Guid.Empty,
                MarketId = s.MarketId,
                Name = s.Name,
                OddValue = s.OddValue,
                SelectionSide = s.Side,
                SelectionId = s.Id
            };
        });
    }
}