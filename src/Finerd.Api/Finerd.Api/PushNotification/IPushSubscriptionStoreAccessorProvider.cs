namespace Finerd.Api.PushNotification
{
    public interface IPushSubscriptionStoreAccessorProvider
    {
        IPushSubscriptionStoreAccessor GetPushSubscriptionStoreAccessor();
    }
}
