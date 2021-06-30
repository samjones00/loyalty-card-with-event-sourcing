using System;

namespace LoyaltyCard.Core.Extensions
{
    public static class StreamExtensions
    {
        public static string GetStreamName<T>(Guid aggregateId) => $"{typeof(T).FullName}-{aggregateId}";
    }
}
