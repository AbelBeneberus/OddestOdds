using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OddestOdds.Data.Database;
using OddestOdds.Data.Models;
using OddestOdds.Data.Repository;
using Xunit;

namespace OddestOdds.UnitTest
{
    public class FixtureRepositoryTests
    {
        private readonly FixtureDbContext _dbContext;
        private readonly Mock<ILogger<FixtureRepository>> _mockLogger;
        private readonly FixtureRepository _fixtureRepository;

        public FixtureRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FixtureDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new FixtureDbContext(options);
            _mockLogger = new Mock<ILogger<FixtureRepository>>();
            _fixtureRepository = new FixtureRepository(_dbContext, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateFixtureAsync_ShouldAddFixture()
        {
            // Arrange
            var fixture = new Fixture
            {
                Id = Guid.NewGuid(),
                FixtureName = "Test Fixture",
                HomeTeam = "Home Team",
                AwayTeam = "Away Team"
            };

            // Act
            await _fixtureRepository.CreateFixtureAsync(fixture);

            // Assert
            _dbContext.Fixtures.Should().Contain(fixture);
        }
    }
}