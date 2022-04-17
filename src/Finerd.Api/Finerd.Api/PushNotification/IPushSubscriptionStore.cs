namespace Finerd.Api.PushNotification
{
    public interface IPushSubscriptionStore
    {
        Task StoreSubscriptionAsync(PushSubscription subscription);

        Task ForEachSubscriptionAsync(Action<PushSubscription> action);

        Task DiscardSubscriptionAsync(string endpoint);
    }
}
