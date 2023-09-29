using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OddestOdds.Common.Models;
using OddestOdds.Data.Enums;
using OddestOdds.Data.Models;
using OddestOdds.IntegrationTest.Drivers;
using Xunit;

namespace OddestOdds.IntegrationTest.Steps;

[Binding]
public class BackOfficeOddsManagementSteps
{
    private readonly HttpClient _httpClient;
    private readonly ScenarioContext _scenarioContext;
    private readonly HandlerDriver _handlerDriver;
    private readonly SignalRDriver _signalRDriver;

    private const string FixtureIdKey = "FixtureId";
    private const string MarketIdKey = "MarketId";
    private const string MarketSelectionIdKey = "MarketSelectionId";
    private const string MarketSelectionNameIdentifierKey = "MarketSelectionName";
    private const string UpdateOddRequestKey = "UpdateOddRequest";

    public BackOfficeOddsManagementSteps(ScenarioContext scenarioContext,
        HandlerDriver handlerDriver,
        SignalRDriver signalRDriver)
    {
        _scenarioContext = scenarioContext;
        _handlerDriver = handlerDriver;
        _httpClient = handlerDriver.GetClient();
        _signalRDriver = signalRDriver;
    }

    [Given(@"I have the details for a new odd")]
    public void GivenIHaveTheDetailsForANewOdd()
    {
        var request = CreateOddRequest(Guid.NewGuid());

        _scenarioContext["CreateOddRequest"] = request;
    }

    [When(@"I send a POST request to the 'Create Odd' endpoint")]
    public async Task WhenISendPostRequestToTheCreateOddsEndpoint()
    {
        var request = _scenarioContext.Get<CreateOddRequest>("CreateOddRequest");

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/odds", jsonContent);
        _scenarioContext.Set(response, "CreateOddResponse");
    }

    [Then(@"the response should confirm the odds have been created")]
    public void ThenTheResponseShouldConfirmTheOddsHaveBeenCreated()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("CreateOddResponse");
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Given(@"I have an existing odd ID and updated details")]
    public async Task GivenIHaveAnExistingOddIdAndUpdatedDetails()
    {
        var selection = await CreateMarketSelectionAndGet();

        var request = new UpdateOddRequest
        {
            MarketSelectionId = selection.Id,
            NewOddValue = 2.5M
        };

        _scenarioContext.Set(request, UpdateOddRequestKey);
    }

    [When(@"I send a PUT request to the 'Update Odds' endpoint")]
    public async Task WhenISendAPutRequestToTheUpdateOddsEndpoint()
    {
        var request = _scenarioContext.Get<UpdateOddRequest>(UpdateOddRequestKey);

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync("/odds", jsonContent);

        _scenarioContext.Set(response, "OddUpdateResponse");
    }

    [Then(@"the response should confirm the odds have been updated")]
    public async Task ThenTheResponseShouldConfirmTheOddsHaveBeenUpdated()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("OddUpdateResponse");
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var name = _scenarioContext.Get<string>(MarketSelectionNameIdentifierKey);

        var selection = await _handlerDriver.GetCreatedOddAsync(name);
        var request = _scenarioContext.Get<UpdateOddRequest>(UpdateOddRequestKey);

        selection.Should().NotBeNull();
        selection.Odd.Should().Be(request.NewOddValue);
    }


    [Given(@"I have an existing odd ID")]
    public async Task GivenIHaveAnExistingOddId()
    {
        var selection = await CreateMarketSelectionAndGet();
        _scenarioContext.Set(selection.Id, MarketSelectionIdKey);
    }


    [When(@"I send a DELETE request to the 'Delete Odds' endpoint")]
    public async Task WhenISendADeleteRequestToTheDeleteOddsEndpoint()
    {
        var existingOddId = _scenarioContext.Get<Guid>(MarketSelectionIdKey);

        var response =
            await _httpClient.DeleteAsync($"/odds?correlationId={Guid.NewGuid()}&marketSelectionId={existingOddId}");

        response.EnsureSuccessStatusCode();
        _scenarioContext.Set(response, "DeleteResponse");
    }

    [Then(@"the response should confirm the odd has been deleted")]
    public void ThenTheResponseShouldConfirmTheOddHasBeenDeleted()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("DeleteResponse");
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Given(@"I have pushAllOdd Request")]
    public void GivenIHavePushAllOddRequest()
    {
        var pushAllOddsRequest = new PushOddsRequest()
        {
            PushAll = true
        };

        _scenarioContext.Set(pushAllOddsRequest, "PushAllOddRequest");
    }

    [Given(@"the SignalR client is ready")]
    public async Task GivenTheSignalRClientIsReady()
    {
        await _signalRDriver.SetupClient(_handlerDriver.GetClient(), _handlerDriver.GetServer());
    }

    [When(@"I send a POST request to the 'Publish Odds' endpoint")]
    public async Task WhenISendAPostRequestToThePublishOddsEndpoint()
    {
        var request = _scenarioContext.Get<PushOddsRequest>("PushAllOddRequest");

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/odds/pushOdds", jsonContent);

        _scenarioContext.Set(response, "OddPublishedResponse");
    }

    [Then(@"the response should confirm the odd has been published")]
    public void ThenTheResponseShouldConfirmTheOddHasBeenPublished()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("OddPublishedResponse");
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Then(@"I should receive the updated odd via SignalR")]
    public void ThenIShouldReceiveTheUpdatedOddViaSignalR()
    {
        var message = _signalRDriver.LastReceivedMessage;
        message.Should().NotBeNull();
    }

    [Given(@"a Fixture with its associated Market is already created")]
    public async Task GivenAFixtureIsAlreadyCreated()
    {
        var marketId = Guid.NewGuid();
        var fixtureId = Guid.NewGuid();
        var createFixtureRequest = new CreateFixtureRequest()
        {
            FixtureName = $"Test fixture {fixtureId}",
            AwayTeam = "Test Away",
            HomeTeam = "Test Home",
            Markets = new List<CreateFixtureRequest.MarketRequest>()
            {
                new CreateFixtureRequest.MarketRequest()
                {
                    MarketName = $"Test market {marketId}"
                }
            }
        };

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(createFixtureRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/fixture", jsonContent);
        response.EnsureSuccessStatusCode();

        var responseContent = JObject.Parse(await response.Content.ReadAsStringAsync());
        responseContent.Should().NotBeNull();
        var data = responseContent["data"].ToString()!;

        var result = JsonConvert.DeserializeObject<FixtureCreatedResponse>(data);

        // Assertion
        result.Should().NotBeNull();
        result.FixtureId.Should().NotBe(Guid.Empty);
        result.MarketIds.Should().NotBeEmpty();

        _scenarioContext.Set(result.FixtureId, FixtureIdKey);
        _scenarioContext.Set(result.MarketIds.First(), MarketIdKey);
    }

    private CreateOddRequest CreateOddRequest(Guid nameId)
    {
        var fixtureId = _scenarioContext.Get<Guid>(FixtureIdKey);
        var marketId = _scenarioContext.Get<Guid>(MarketIdKey);

        var request = new CreateOddRequest
        {
            FixtureId = fixtureId,
            MarketId = marketId,
            SelectionName = $"Test Selection - {nameId}",
            OddValue = 1.5M,
            Side = MarketSelectionSide.Home
        };
        return request;
    }

    private async Task<MarketSelection> CreateMarketSelectionAndGet()
    {
        var nameId = Guid.NewGuid();

        var request = CreateOddRequest(nameId);

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/odds", jsonContent);
        response.EnsureSuccessStatusCode();
        var name = $"Test Selection - {nameId}";
        var selection = await _handlerDriver.GetCreatedOddAsync(name);
        selection.Should().NotBeNull();
        selection.Name.Should().BeEquivalentTo(name);
        _scenarioContext.Set(name, MarketSelectionNameIdentifierKey);
        return selection;
    }
}