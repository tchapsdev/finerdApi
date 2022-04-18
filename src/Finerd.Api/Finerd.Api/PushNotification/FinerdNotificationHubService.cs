using Finerd.Api.Services.Push;

namespace Finerd.Api.PushNotification
{
    public class FinerdNotificationHubService: BackgroundService
    {
        private readonly ILogger<FinerdNotificationHubService> _logger;
        private IServiceScopeFactory _serviceScopeFactory { get; }

        public FinerdNotificationHubService(ILogger<FinerdNotificationHubService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("U")} - Sending Finerd NotificationHubService to user");
                var message = "Thank you for using Finerd App. This aplication is free with Ads!";
                var messageToSend = new Lib.Net.Http.WebPush.PushMessage(message);
                using var scope = _serviceScopeFactory.CreateScope();
                var notifyService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();
                var storeService = scope.ServiceProvider.GetRequiredService<IPushSubscriptionStore>();
                storeService.Query()
                                .ToList()
                                .ForEach(async x => {
                                    _logger.LogInformation($"{x.UserId} - {message}");
                                    await notifyService.SendNotificationAsync(x, messageToSend);
                                });
                await Task.Delay(1000 * 60 * 1);
            }
        }
    }
}
