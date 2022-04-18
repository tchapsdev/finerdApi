using Lib.Net.Http.WebPush;
namespace Finerd.Api.PushNotification
{
    public class PushSubscription: Lib.Net.Http.WebPush.PushSubscription
    {
        public string? P256DH
        {
            get { return GetKey(PushEncryptionKeyName.P256DH); }

            set { SetKey(PushEncryptionKeyName.P256DH, value); }
        }

        public string? Auth
        {
            get { return GetKey(PushEncryptionKeyName.Auth); }

            set { SetKey(PushEncryptionKeyName.Auth, value); }
        }

        public PushSubscription()
        { }

        public PushSubscription(PushSubscription subscription)
        {
            Endpoint = subscription.Endpoint;
            Keys = subscription.Keys;
        }

        public string? UserId { get; set; }
    }
}
