using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OddestOdds.Business.Services;
using OddestOdds.Common.Models;
using OddestOdds.HandlerApp.SwaggerExamples;
using Swashbuckle.AspNetCore.Filters;

namespace OddestOdds.HandlerApp.Controllers;

[ApiController]
[Route("[controller]")]
public class FixtureController : ControllerBase
{
    private readonly ILogger<FixtureController> _logger;
    private readonly IOddService _oddService;

    public FixtureController(ILogger<FixtureController> logger, IOddService oddService)
    {
        _logger = logger;
        _oddService = oddService;
    }

    [HttpPost]
    [SwaggerRequestExample(typeof(CreateFixtureRequest), typeof(CreateFixtureRequestExampleProvider))]
    public async Task<IActionResult> CreateFixture([FromBody] CreateFixtureRequest request)
    {
        try
        {
            var result = await _oddService.CreateFixtureAsync(request);
            return Accepted(new { Message = "Fixture Created Successfully.", Data = result });
        }
        catch (ValidationException exception)
        {
            _logger.LogError("Validation Failed", exception);
            var errorMessages = exception.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Validation Failed", errors = errorMessages });
        }
        catch (Exception e)
        {
            _logger.LogError("Unhandled Exception", e);
            return StatusCode(500, "Internal server error occured while creating fixture");
        }
    }
}