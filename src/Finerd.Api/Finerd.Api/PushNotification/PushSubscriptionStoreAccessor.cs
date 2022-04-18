namespace Finerd.Api.PushNotification
{
    public class PushSubscriptionStoreAccessor : IPushSubscriptionStoreAccessor
    {
        public IPushSubscriptionStore PushSubscriptionStore { get; private set; }

        public PushSubscriptionStoreAccessor(IPushSubscriptionStore pushSubscriptionStore)
        {
            PushSubscriptionStore = pushSubscriptionStore;
        }

        public void Dispose()
        { }
    }
}
