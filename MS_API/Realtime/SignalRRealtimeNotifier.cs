using Microsoft.AspNetCore.SignalR;
using MS_Application.Services.Interfaces.External;

namespace MS_API.Realtime
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRRealtimeNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyUserAsync(long userId, string eventName, object payload)
        {
            return _hubContext.Clients
                .User(userId.ToString())
                .SendAsync(eventName, payload);
        }
    }
}
