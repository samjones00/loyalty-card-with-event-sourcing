using System;
using System.Collections.Generic;

namespace LoyaltyCard.Domain.Interfaces
{
    public interface IAggregate
    {
        Guid Id { get; }
        long Version { get; }
        void Apply(object e);
        void Load(long version, IEnumerable<object> history);
        object[] GetChanges();
    }
}