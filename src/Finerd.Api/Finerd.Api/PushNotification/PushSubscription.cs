namespace Finerd.Api.PushNotification
{
    public class PushSubscription
    {
        public string Endpoint { get; set; }

        public IDictionary<string, string> Keys { get; set; }
    }
}
