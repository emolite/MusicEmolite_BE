using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MS_API.Realtime
{
    /// <summary>
    /// Pure server-to-client push channel for chat messages and friend events.
    /// Clients never call methods on this hub - they just connect and listen.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
    }
}
