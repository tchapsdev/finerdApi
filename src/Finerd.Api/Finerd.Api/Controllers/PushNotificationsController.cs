using Finerd.Api.PushNotification;
using Finerd.Api.Services;
using Finerd.Api.Services.Push;
using Microsoft.AspNetCore.Mvc;

//https://www.tpeczek.com/2017/12/push-notifications-and-aspnet-core-part.html
namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            _logger.LogInformation($"{DateTime.Now.ToString("U")} - UserID ({UserID}) Sending Finerd NotificationHubService to user");
            return NoContent();
        }

        // DELETE push-notifications-api/subscriptions?endpoint={endpoint}
        [HttpDelete("subscriptions")]
        public async Task<IActionResult> DiscardSubscription(string endpoint)
        {
            await _subscriptionStore.DiscardSubscriptionAsync(endpoint);
            return NoContent();
        }

        // DELETE push-notifications-api/subscriptions?endpoint={endpoint}
        [HttpPost("subscriptions/sendMessage")]
        public async Task<IActionResult> SendNotification(string message)
        {
            var messageToSend = new Lib.Net.Http.WebPush.PushMessage(message);
            _subscriptionStore.Query()
                            .ToList()
                            .ForEach(async x => await _pushNotificationService.SendNotificationAsync(x, messageToSend));
            return NoContent();
        }
    }

   
}
