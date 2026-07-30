namespace MS_Application.Services.Interfaces.External
{
    public interface IRealtimeNotifier
    {
        Task NotifyUserAsync(long userId, string eventName, object payload);
    }
}
