using Microsoft.AspNetCore.SignalR;

namespace MS_API.Realtime
{
    /// <summary>
    /// Maps a hub connection to our own "UserId" JWT claim instead of the
    /// default ClaimTypes.NameIdentifier (which this app never sets), so
    /// Clients.User(userId) resolves correctly.
    /// </summary>
    public class ChatHubUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("UserId")?.Value;
        }
    }
}
