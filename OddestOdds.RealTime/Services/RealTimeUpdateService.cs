using Microsoft.AspNetCore.SignalR;
using OddestOdds.Common.Messages;

namespace OddestOdds.RealTime.Services;

public class RealTimeUpdateService : IRealTimeUpdateService<OddUpdatedMessage>
{
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly ILogger<RealTimeUpdateService> _logger;

    public RealTimeUpdateService(IHubContext<MessageHub> hubContext, ILogger<RealTimeUpdateService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendOddsUpdateAsync(OddUpdatedMessage message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveOddsUpdate", message);
    }
}