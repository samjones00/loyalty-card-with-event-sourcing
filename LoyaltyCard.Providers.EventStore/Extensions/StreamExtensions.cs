using System;

namespace LoyaltyCard.Providers.EventStore.Extensions
{
    public static class StreamExtensions
    {
        public static string GetStreamName<T>(Guid aggregateId) => $"{typeof(T).FullName}-{aggregateId}";
    }
}
