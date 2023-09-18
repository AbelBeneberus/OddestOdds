using Microsoft.AspNetCore.SignalR.Client;
using OddestOdds.Common.Messages;

namespace OddestOdds.PunterApp;

public static class Program
{
    private static HubConnection? _hubConnection;
    private const string BaseUrl = "http://localhost:51642/";

    static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();
        Console.WriteLine("Welcome to GIG Punter APP. Press 'q' to quit.");

        _ = Task.Run(() =>
        {
            while (Console.ReadKey().KeyChar != 'q')
            {
                var cancellationToken = CancellationToken.None;
            }
        }, cts.Token);

        await SetupRealtimeUpdateReceiver();

        using HttpClient httpClient = new HttpClient();
        try
        {
            HttpResponseMessage response =
                await httpClient.GetAsync($"{BaseUrl}odds?correlationId={Guid.NewGuid()}&asTree={bool.TrueString}",
                    cts.Token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(CancellationToken.None);
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        try
        {
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Application Exited!");
        }

        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync(CancellationToken.None);
        }
    }

    private static async Task SetupRealtimeUpdateReceiver()
    {
        var httpClientHandler = new HttpClientHandler();

        httpClientHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        _hubConnection = new HubConnectionBuilder().WithUrl($"{BaseUrl}messageHub",
                options => { options.HttpMessageHandlerFactory = _ => httpClientHandler; })
            .WithAutomaticReconnect()
            .Build();
        _hubConnection.On<OddUpdatedMessage>("ReceiveOddsUpdate",
            update => { Console.WriteLine(update.ToString()); });

        _hubConnection.On<OddCreatedMessage>("ReceiveOddCreation",
            newOdd => { Console.WriteLine(newOdd.ToString()); });

        _hubConnection.On<OddDeletedMessage>("ReceiveOddDeleted",
            deletedOdd => { Console.WriteLine(deletedOdd.ToString()); });

        _hubConnection.On<PushedOddMessage>("ReceiveOddPushed",
            pushedOdd => { Console.WriteLine(pushedOdd.ToString()); });

        await _hubConnection.StartAsync();
    }
}