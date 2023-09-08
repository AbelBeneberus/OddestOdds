using Microsoft.AspNetCore.SignalR;
using OddestOdds.Common.Messages;

namespace OddestOdds.RealTime;

public class MessageHub : Hub
{
    public async Task SendOddsUpdate(Message update)
    {
        await Clients.All.SendAsync("ReceiveOddsUpdate", update);
    }
}