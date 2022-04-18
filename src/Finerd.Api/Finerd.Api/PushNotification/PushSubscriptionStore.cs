using Finerd.Api.Data;

namespace Finerd.Api.PushNotification
{
    internal class PushSubscriptionStore : IPushSubscriptionStore
    {
        public PushSubscriptionStore()
        {           
        }

        public Task StoreSubscriptionAsync(PushSubscription subscription)
        {
            using (var db = new ApplicationDbContext())
            {
                if (!db.PushSubscriptions.Any(p => p.Endpoint == subscription.Endpoint))
                {
                    db.Add(subscription);
                    db.SaveChanges();
                }
            }                  
            return Task.CompletedTask;
        }

        public Task DiscardSubscriptionAsync(string endpoint)
        {
            using (var db = new ApplicationDbContext())
            {
                if (db.PushSubscriptions.Any(p => p.Endpoint == endpoint))
                {
                    db.Remove(endpoint);
                    db.SaveChanges();
                }
            }                    
            return Task.CompletedTask;
        }

        public Task ForEachSubscriptionAsync(Action<PushSubscription> action)
        {
            return ForEachSubscriptionAsync(action, CancellationToken.None);
        }

        public IQueryable<PushSubscription> Query()
        {
            using var db = new ApplicationDbContext();
            return db.PushSubscriptions.AsQueryable();
        }

        public Task ForEachSubscriptionAsync(Action<PushSubscription> action, CancellationToken cancellationToken)
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (PushSubscription subscription in db.PushSubscriptions.ToList())
                {
                    action(subscription);
                }
            }           
            return Task.CompletedTask;
        }

        public Task ForEachSubscriptionAsync<T>(Action<PushSubscription, T> action, T param, CancellationToken cancellationToken) where T :class
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (PushSubscription subscription in db.PushSubscriptions.ToList())
                {
                    action(subscription, param);
                }
            }           
            return Task.CompletedTask;
        }
    }
}
