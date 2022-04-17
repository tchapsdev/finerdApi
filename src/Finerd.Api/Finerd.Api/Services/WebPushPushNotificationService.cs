using Finerd.Api.Hubs;
using Finerd.Api.PushNotification;
using Microsoft.Extensions.Options;
using WebPush;

namespace Finerd.Api.Services
{
    public class WebPushPushNotificationService : IPushNotificationService
    {
        private readonly PushNotificationServiceOptions _options;
        private readonly WebPushClient _pushClient;

        public string PublicKey { get { return _options.PublicKey; } }

        public WebPushPushNotificationService(IOptions<PushNotificationServiceOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;

            _pushClient = new WebPushClient();
            _pushClient.SetVapidDetails(_options.Subject, _options.PublicKey, _options.PrivateKey);
        }

        public async Task SendNotification(PushNotification.PushSubscription subscription, string payload)
        {
            //var webPushSubscription = WebPush.PushSubscription(
            //    subscription.Endpoint,
            //    subscription.Keys["p256dh"],
            //    subscription.Keys["auth"]);

            //_pushClient.SendNotification(webPushSubscription, payload);

            //var pushEndpoint = @"https://fcm.googleapis.com/fcm/send/efz_TLX_rLU:APA91bE6U0iybLYvv0F3mf6uDLB6....";
            //var p256dh = @"BKK18ZjtENC4jdhAAg9OfJacySQiDVcXMamy3SKKy7FwJcI5E0DKO9v4V2Pb8NnAPN4EVdmhO............";
            //var auth = @"fkJatBBEl...............";

            //var subject = @"mailto:example@example.com";
            //var publicKey = @"BDjASz8kkVBQJgWcD05uX3VxIs_gSHyuS023jnBoHBgUbg8zIJvTSQytR8MP4Z3-kzcGNVnM...............";
            //var privateKey = @"mryM-krWj_6IsIMGsd8wNFXGBxnx...............";

            var webPushSubscription = new  WebPush.PushSubscription(subscription.Endpoint, subscription.Keys["p256dh"], subscription.Keys["auth"]);
            //var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
            //var gcmAPIKey = @"[your key here]";

            var webPushClient = new WebPushClient();
            try
            {
                await webPushClient.SendNotificationAsync(webPushSubscription, payload);
                //await webPushClient.SendNotificationAsync(webPushSubscription, payload, vapidDetails);
                //await webPushClient.SendNotificationAsync(subscription, "payload", gcmAPIKey);
            }
            catch (WebPushException exception)
            {
                Console.WriteLine("Http STATUS code" + exception.StatusCode);
            }
        }
    }
}
