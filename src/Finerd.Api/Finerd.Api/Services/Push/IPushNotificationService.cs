using PushSubscription = Lib.Net.Http.WebPush.PushSubscription;

namespace Finerd.Api.Services.Push
{
    public interface IPushNotificationService
    {
        string PublicKey { get; }

        Task SendNotificationAsync(PushSubscription subscription, Lib.Net.Http.WebPush.PushMessage message);

        Task SendNotificationAsync(PushSubscription subscription, Lib.Net.Http.WebPush.PushMessage message, CancellationToken cancellationToken);
    }
}
