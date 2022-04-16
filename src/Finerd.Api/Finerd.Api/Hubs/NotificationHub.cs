using Microsoft.AspNetCore.SignalR;

namespace Finerd.Api.Hubs
{
    // https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-6.0
    // https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr-typescript-webpack?view=aspnetcore-6.0&tabs=visual-studio
    public class NotificationHub : Hub<INotificationClient>
    {
        public async Task SendMessage(string user, string message) => await Clients.All.ReceiveMessage(user, message);

        public async Task SendMessageToCaller(string user, string message) => await Clients.Caller.ReceiveMessage(user, message);

        public async Task SendMessageToGroup(string user, string message) => await Clients.Group("Finerd Users").ReceiveMessage(user, message);


        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Finerd Users");
            var userName = "Guest";
            if (Context.User?.Identity != null &&  Context.User.Identity.IsAuthenticated)
            {
                userName = $"{Context.User?.Identity?.Name} ({Context.UserIdentifier})";
            }
            await Clients.Caller.ReceiveMessage(userName, $"subscribe to Finerd's Hub Broascasting push notification");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Finerd Users");
            await base.OnDisconnectedAsync(exception);
        }

        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
    }
}
