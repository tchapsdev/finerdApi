using Finerd.Api.Hubs;
using Finerd.Api.PushNotification;

namespace Finerd.Api.Services
{
    public interface IPushNotificationService
    {
        Task SendNotification(PushSubscription subscription, string payload);
    }
}
