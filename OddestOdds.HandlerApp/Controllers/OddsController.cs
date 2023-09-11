using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OddestOdds.Business.Services;
using OddestOdds.Common.Exceptions;
using OddestOdds.Common.Models;

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
}