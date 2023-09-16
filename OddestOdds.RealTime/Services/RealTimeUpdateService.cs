using Microsoft.AspNetCore.SignalR;
using OddestOdds.Common.Messages;

namespace OddestOdds.RealTime.Services;

public class RealTimeUpdateService : IRealTimeUpdateService<OddUpdatedMessage>,
    IRealTimeUpdateService<OddCreatedMessage>,
    IRealTimeUpdateService<OddDeletedMessage>
{
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly ILogger<RealTimeUpdateService> _logger;

    public RealTimeUpdateService(IHubContext<MessageHub> hubContext, ILogger<RealTimeUpdateService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task HandleMessageAsync(OddUpdatedMessage message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveOddsUpdate", message);
    }

    public async Task HandleMessageAsync(OddCreatedMessage message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveOddCreation", message);
    }

    public async Task HandleMessageAsync(OddDeletedMessage message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveOddDeleted", message);
    }
}