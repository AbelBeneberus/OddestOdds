using OddestOdds.Common.Models;

namespace OddestOdds.Business.Services;

public interface IOddService
{
    Task<FixtureCreatedResponse> CreateFixtureAsync(CreateFixtureRequest request);
    Task CreateOddAsync(CreateOddRequest request);
    Task UpdateOddAsync(UpdateOddRequest request);
}