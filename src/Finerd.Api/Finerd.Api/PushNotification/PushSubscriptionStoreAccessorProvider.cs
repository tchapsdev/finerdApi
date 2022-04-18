namespace Finerd.Api.PushNotification
{
    public class PushSubscriptionStoreAccessorProvider : IPushSubscriptionStoreAccessorProvider
    {
        private readonly IPushSubscriptionStore _pushSubscriptionStore;

        public PushSubscriptionStoreAccessorProvider(IPushSubscriptionStore pushSubscriptionStore)
        {
            _pushSubscriptionStore = pushSubscriptionStore;
        }

        public IPushSubscriptionStoreAccessor GetPushSubscriptionStoreAccessor()
        {
            return new PushSubscriptionStoreAccessor(_pushSubscriptionStore);
        }
    }
}
