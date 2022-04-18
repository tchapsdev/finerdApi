using Finerd.Api.PushNotification;
using Finerd.Api.Services;
using Finerd.Api.Services.Push;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

// https://www.tpeczek.com/2017/12/push-notifications-and-aspnet-core-part.html
// https://github.com/web-push-libs/web-push-csharp
namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PushNotificationsController : BaseApiController
    {
        private readonly IPushSubscriptionStore _subscriptionStore;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ILogger<PushNotificationsController> _logger;

        public PushNotificationsController(IPushSubscriptionStore subscriptionStore, IPushNotificationService pushNotificationService, 
            ILogger<PushNotificationsController> logger)
        {
            _subscriptionStore = subscriptionStore;
            _pushNotificationService = pushNotificationService;
            _logger = logger;
        }

        // POST push-notifications-api/subscriptions
        [HttpPost("subscriptions")]
        public async Task<IActionResult> StoreSubscription([FromBody] PushSubscription subscription)
        {
            subscription.UserId = UserID.ToString();
            await _subscriptionStore.StoreSubscriptionAsync(subscription);
            _logger.LogInformation($@"{DateTime.Now.ToString("U")} - StoreSubscription. UserID ({UserID}) Subscription to Finerd NotificationHubService
                                            subscription: {JsonConvert.SerializeObject(subscription)}");
            return NoContent();
        }

        // DELETE push-notifications-api/subscriptions?endpoint={endpoint}
        [HttpDelete("subscriptions")]
        public async Task<IActionResult> DiscardSubscription(string endpoint)
        {
            _logger.LogInformation($@"{DateTime.Now.ToString("U")} - DiscardSubscription. UserID ({UserID}) UnSubscription from Finerd NotificationHubService
                                        endpoint: {endpoint}");
            await _subscriptionStore.DiscardSubscriptionAsync(endpoint);
            return NoContent();
        }

        // DELETE push-notifications-api/subscriptions?endpoint={endpoint}
        [HttpPost("subscriptions/sendMessage")]
        public async Task<IActionResult> SendNotification(string message)
        {
            var data = new
            {
                Title = "Thank you for choosing finerd",
                Message = message
            };
            var ContentMessage = new StringContent($"{JsonConvert.SerializeObject(data)}",
                                    Encoding.UTF8,
                                    "application/json");//CONTENT-TYPE header;
            var messageToSend = new Lib.Net.Http.WebPush.PushMessage(ContentMessage);
            messageToSend.Topic = "Thank you for choosing finerd";
            _logger.LogInformation($@"{DateTime.Now.ToString("U")} - SendNotification. UserID ({UserID}) Sending Finerd NotificationHubService to all user
                                    message: {JsonConvert.SerializeObject(messageToSend)}");
            await _subscriptionStore.ForEachSubscriptionAsync(
                        async (PushSubscription subscription) => await _pushNotificationService.SendNotificationAsync(subscription, messageToSend)
                    );            
            return NoContent();
        }
    }

   
}
