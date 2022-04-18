using Microsoft.AspNetCore.SignalR;

namespace Finerd.Api.Hubs
{
    public class FinerdHubService: BackgroundService
    {
        private readonly ILogger<FinerdHubService> _logger;
        private readonly IHubContext<NotificationHub, INotificationClient> _notificationHubContext;
        public FinerdHubService(
            ILogger<FinerdHubService> logger,  IHubContext<NotificationHub, INotificationClient> notificationHubContext)
        {
            _logger = logger;
            _notificationHubContext = notificationHubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Sending message to GUEST user to login"); 
                 await _notificationHubContext.Clients.Group("Guest").ReceiveMessage("Finerds", "Please SignUp or SignIn to use your lifetime free account!");
                await Task.Delay(1000 * 60 * 5);                
            }
        }
    }
}
