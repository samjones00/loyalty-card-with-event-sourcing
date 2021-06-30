using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoyaltyCard.Core.Interfaces
{
    public interface IAggregateStore
    {
        Task<T> Load<T>(Guid userId, CancellationToken cancellationToken = default) where T : Aggregate, new();
        Task<(long NextExpectedVersion, long LogPosition, long CommitPosition)> Save<T>(T aggregate, CancellationToken cancellationToken = default) where T : Aggregate;
    }
}
