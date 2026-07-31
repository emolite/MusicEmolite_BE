using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MS_API.Realtime
{
    /// <summary>
    /// Server-to-client push channel for chat messages and friend events, plus
    /// the one thing that has to go the other way: the typing indicator, since
    /// nothing gets persisted for it - it's a pure client -> hub -> client relay.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task Typing(long receiverId)
        {
            var senderId = Context.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            await Clients.User(receiverId.ToString())
                .SendAsync("UserTyping", new { senderId = long.Parse(senderId) });
        }
    }
}
