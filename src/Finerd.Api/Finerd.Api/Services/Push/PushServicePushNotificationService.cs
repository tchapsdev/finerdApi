using Finerd.Api.PushNotification;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Options;
using System.Net;
using PushSubscription = Lib.Net.Http.WebPush.PushSubscription;

namespace Finerd.Api.Services.Push
{
    internal class PushServicePushNotificationService : IPushNotificationService
    {
        private  PushServiceClient _pushClient;
        private readonly IPushSubscriptionStoreAccessorProvider _subscriptionStoreAccessorProvider;
        private readonly ILogger _logger;
        private readonly PushNotificationServiceOptions _options;

        public string PublicKey { get { return _pushClient.DefaultAuthentication.PublicKey; } }

        public PushServicePushNotificationService( IPushSubscriptionStoreAccessorProvider subscriptionStoreAccessorProvider, 
            ILogger<PushServicePushNotificationService> logger, IOptions<PushNotificationServiceOptions> optionsAccessor)
        {
           // _pushClient = pushClient;
            _subscriptionStoreAccessorProvider = subscriptionStoreAccessorProvider;
            _logger = logger;
            _options = optionsAccessor.Value;
            _pushClient = new PushServiceClient();
            _pushClient.DefaultAuthentication = new Lib.Net.Http.WebPush.Authentication.VapidAuthentication(_options.PublicKey, _options.PrivateKey);
        }

        public Task SendNotificationAsync(PushSubscription subscription, PushMessage message)
        {
            return SendNotificationAsync(subscription, message, CancellationToken.None);
        }

        public async Task SendNotificationAsync(PushSubscription subscription, PushMessage message, CancellationToken cancellationToken)
        {
            try
            {
                //_pushClient = new PushServiceClient();
                await _pushClient.RequestPushMessageDeliveryAsync(subscription, message, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandlePushMessageDeliveryExceptionAsync(ex, subscription);
            }
        }

        private async Task HandlePushMessageDeliveryExceptionAsync(Exception exception, PushSubscription subscription)
        {
            PushServiceClientException pushServiceClientException = exception as PushServiceClientException;

            if (pushServiceClientException is null)
            {
                _logger?.LogError(exception, "Failed requesting push message delivery to {0}.", subscription.Endpoint);
            }
            else
            {
                if ((pushServiceClientException.StatusCode == HttpStatusCode.NotFound) || (pushServiceClientException.StatusCode == HttpStatusCode.Gone))
                {
                    using (IPushSubscriptionStoreAccessor subscriptionStoreAccessor = _subscriptionStoreAccessorProvider.GetPushSubscriptionStoreAccessor())
                    {
                        await subscriptionStoreAccessor.PushSubscriptionStore.DiscardSubscriptionAsync(subscription.Endpoint);
                    }
                    _logger?.LogInformation("Subscription has expired or is no longer valid and has been removed.");
                }
            }
        }
    }
}
