using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using OddestOdds.Common.Messages;

namespace OddestOdds.IntegrationTest.Drivers;

[Binding]
public class SignalRDriver
{
    private HubConnection _hubConnection;
    public PushedOddMessage LastReceivedMessage { get; private set; }

    public async Task SubscribeAsync(Guid fixtureId)
    {
        await _hubConnection.InvokeAsync("SubscribeToFixture", fixtureId);
    }

    public PushedOddMessage GetLastReceivedMessage()
    {
        return LastReceivedMessage;
    }

    public async Task SetupClient(HttpClient sharedClient, TestServer server)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(
                $"{sharedClient.BaseAddress}messageHub",
                o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
            .Build();
        _hubConnection.On<PushedOddMessage>("ReceiveOddPushed", message => { LastReceivedMessage = message; });
        await _hubConnection.StartAsync();
    }
}