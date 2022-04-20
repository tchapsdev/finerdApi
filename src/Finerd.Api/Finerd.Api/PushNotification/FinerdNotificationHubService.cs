using Finerd.Api.Data;
using Finerd.Api.Services.Push;
using Newtonsoft.Json;
using System.Text;

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
                try
                {
                    await Task.Delay(1000 * 60 * 15);
                    _logger.LogInformation($"{DateTime.Now.ToString("U")} - Sending Finerd NotificationHubService to user");
                    var message = "Do not forget to Add your last daily transactions.";
                    var data = new
                    {
                        Title = "Reminder",
                        Message = message
                    };
                    var messageToSend = new Lib.Net.Http.WebPush.PushMessage(JsonConvert.SerializeObject(data));
                    messageToSend.Topic = "Login notification";
                    using var scope = _serviceScopeFactory.CreateScope();
                    var notifyService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();
                    var storeService = scope.ServiceProvider.GetRequiredService<IPushSubscriptionStore>();
                    List<PushSubscription> pushSubscriptions = storeService.Query().ToList();
                   
                    pushSubscriptions.Where(s => s.UserId != "-1").ToList().ForEach(async x => {
                        _logger.LogInformation($"{x.UserId} - {message}");
                        await notifyService.SendNotificationAsync(x, messageToSend);
                    });

#if DEBUG
                    await Task.Delay(1000 * 60 * 15);
#else
                await Task.Delay(1000 * 60 * 60);
#endif
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
               
            }
        }
    }
}
