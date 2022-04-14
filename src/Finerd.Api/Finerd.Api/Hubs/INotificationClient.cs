namespace Finerd.Api.Hubs
{
    public interface INotificationClient
    {        
        Task ReceiveMessage(string user, string message);
    }
}
