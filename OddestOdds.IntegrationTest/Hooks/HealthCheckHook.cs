using OddestOdds.IntegrationTest.Drivers;
using Xunit;

namespace OddestOdds.IntegrationTest.Hooks
{
    [Binding]
    public class HealthCheckHook
    {
        private readonly HttpClient _client;

        public HealthCheckHook(ScenarioContext scenarioContext, HandlerDriver handlerDriver)
        {
            _client = handlerDriver.GetClient();
        }

        [BeforeScenario]
        public async Task BeforeFeatureStart()
        {
            var response = await _client.GetAsync("health");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode, $"Health check failed with status code: {response.StatusCode}");
            Assert.Contains("Healthy", responseContent);
        }
    }
}