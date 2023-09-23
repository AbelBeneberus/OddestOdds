using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OddestOdds.Business.Services;
using OddestOdds.Common.Extensions;
using OddestOdds.Common.Models;
using OddestOdds.Data.Exceptions;

namespace OddestOdds.HandlerApp.Controllers;

[ApiController]
[Route("[controller]")]
public class OddsController : ControllerBase
{
    private readonly ILogger<OddsController> _logger;
    private readonly IOddService _oddService;

    public OddsController(ILogger<OddsController> logger, IOddService oddService)
    {
        _logger = logger;
        _oddService = oddService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOdd([FromBody] CreateOddRequest request)
    {
        try
        {
            await _oddService.CreateOddAsync(request);
            return Accepted();
        }
        catch (ValidationException exception)
        {
            _logger.LogError("Validation Failed", exception);
            var errorMessages = exception.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new
            {
                Message = "Validation Failed",
                errors = errorMessages
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Unhandled Exception", e);
            return StatusCode(500, "Internal server error occured while creating odds");
        }
    }

    [HttpPost("pushOdds")]
    public async Task<IActionResult> PushOdds([FromBody] PushOddsRequest request)
    {
        try
        {
            await _oddService.PushOddAsync(request);
            return Ok();
        }
        catch (ValidationException exception)
        {
            _logger.LogError("Validation Failed", exception);
            var errorMessages = exception.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new
            {
                Message = "Validation Failed",
                errors = errorMessages
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Unhandled Exception", e);
            return StatusCode(500, "Internal server error occured while pushing odds");
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOdd([FromBody] UpdateOddRequest request)
    {
        try
        {
            await _oddService.UpdateOddAsync(request);
            return Accepted();
        }
        catch (MarketSelectionNotFoundException exception)
        {
            return NotFound(new
            {
                Message = $"Market selection can not be found for MarketSelectionId: {exception.SelectionId}"
            });
        }
        catch (ValidationException exception)
        {
            _logger.LogError("Validation Failed", exception);
            var errorMessages = exception.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Validation Failed", errors = errorMessages });
        }
        catch (Exception exception)
        {
            _logger.LogError("Unhandled Exception occured while updating MarketSelection", exception);
            return StatusCode(500, "Internal server error occured while updating the odd.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOdds(Guid correlationId, bool asTree)
    {
        try
        {
            var result = await _oddService.GetAllOddsAsync();
            _logger.LogInformation("Correlation Id : {CorrelationId} requested for getting all odds", correlationId);

            return Ok(asTree ? result.ToTreeStructure() : result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Correlation Id : {CorrelationId} Error occured while trying to fetch all odds",
                correlationId);
            return StatusCode(500, $"Internal server error occured while trying to fetch all odds");
        }
    }

    [HttpGet("{fixtureIds}")]
    public async Task<IActionResult> GetOddsByFixtureIds(Guid correlationId, string fixtureIds)
    {
        var parsedFixtureIds = fixtureIds.Split(',').Select(s => Guid.Parse(s));
        try
        {
            var result = await _oddService.GetOddsByFixtureIdsAsync(parsedFixtureIds);
            _logger.LogInformation(
                "Correlation Id : {CorrelationId} requested for getting odds for the following fixtureIds {fixtureIds}",
                correlationId, fixtureIds);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Correlation Id : {CorrelationId} Error occured while trying to fetch odds with fixtureId : {fixtureIds}",
                correlationId, fixtureIds);
            return StatusCode(500, $"Internal server error occured while trying to fetch odds.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOdd(Guid correlationId, Guid marketSelectionId)
    {
        try
        {
            await _oddService.DeleteOddAsync(marketSelectionId);
            _logger.LogInformation(
                "Correlation Id : {CorrelationId} requested for deleting odd for the following marketSelectionId {marketSelectionId}",
                correlationId, marketSelectionId);
            return Ok();
        }
        catch (MarketSelectionNotFoundException exception)
        {
            return NotFound(new
            {
                Message = $"Market selection can not be found for MarketSelectionId: {exception.SelectionId}"
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal server error occured while trying to delete market selection");
            return StatusCode(500, "Internal Server Error Occured");
        }
    }
}