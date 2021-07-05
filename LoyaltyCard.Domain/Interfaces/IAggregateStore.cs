using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoyaltyCard.Domain.Interfaces
{
    public interface IAggregateStore
    {
        Task<T> Load<T>(Guid userId, CancellationToken cancellationToken = default) where T : IAggregate, new();
        Task<(long NextExpectedVersion, long LogPosition, long CommitPosition)> Save<T>(T aggregate, CancellationToken cancellationToken = default) where T : IAggregate;
    }
}
