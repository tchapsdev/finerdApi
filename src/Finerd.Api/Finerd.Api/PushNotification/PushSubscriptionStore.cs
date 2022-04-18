﻿using Finerd.Api.Data;

namespace Finerd.Api.PushNotification
{
    internal class PushSubscriptionStore : IPushSubscriptionStore
    {
        private readonly ApplicationDbContext _database;

        public PushSubscriptionStore(ApplicationDbContext database)
        {
            _database = database;
        }

        public Task StoreSubscriptionAsync(PushSubscription subscription)
        {
            _database.Add(subscription);
            _database.SaveChanges();
            return Task.CompletedTask;
        }

        public Task DiscardSubscriptionAsync(string endpoint)
        {
            _database.Remove(endpoint);
            _database.SaveChanges();
            return Task.CompletedTask;
        }

        public Task ForEachSubscriptionAsync(Action<PushSubscription> action)
        {
            return ForEachSubscriptionAsync(action, CancellationToken.None);
        }

        public IQueryable<PushSubscription> Query()
        {
            return _database.PushSubscriptions.AsQueryable();
        }

        public Task ForEachSubscriptionAsync(Action<PushSubscription> action, CancellationToken cancellationToken)
        {
            foreach (PushSubscription subscription in _database.PushSubscriptions.ToList())
            {
                action(subscription);
            }
            return Task.CompletedTask;
        }

        public Task ForEachSubscriptionAsync<T>(Action<PushSubscription, T> action, T param, CancellationToken cancellationToken) where T :class
        {
            foreach (PushSubscription subscription in _database.PushSubscriptions.ToList())
            {
                action(subscription, param);
            }
            return Task.CompletedTask;
        }
    }
}