namespace Finerd.Api.PushNotification
{
    public interface IPushSubscriptionStoreAccessor : IDisposable
    {
        IPushSubscriptionStore PushSubscriptionStore { get; }
    }
}
