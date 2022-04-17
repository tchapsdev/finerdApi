using Finerd.Api.PushNotification;
using Finerd.Api.Services;
using Microsoft.AspNetCore.Mvc;

//https://www.tpeczek.com/2017/12/push-notifications-and-aspnet-core-part.html
namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushNotificationsController : ControllerBase
    {
        private readonly IPushSubscriptionStore _subscriptionStore;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ILogger<HeathController> _logger;

        public PushNotificationsController(IPushSubscriptionStore subscriptionStore, IPushNotificationService pushNotificationService, ILogger<HeathController> logger)
        {
            _subscriptionStore = subscriptionStore;
            _pushNotificationService = pushNotificationService;
            _logger = logger;
        }

        // POST push-notifications-api/subscriptions
        [HttpPost("subscriptions")]
        public async Task<IActionResult> StoreSubscription([FromBody] PushSubscription subscription)
        {
            await _subscriptionStore.StoreSubscriptionAsync(subscription);
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
        [HttpPost("subscriptions?payload={payload}")]
        public async Task<IActionResult> SendNotification(string payload, [FromBody] PushSubscription subscription)
        {
            await _pushNotificationService.SendNotification(subscription, payload);
            return NoContent();
        }
    }

   
}
