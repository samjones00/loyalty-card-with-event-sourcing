using System;
using LoyaltyCard.Domain.Interfaces;

namespace LoyaltyCard.Domain.Events
{
    public class UserDeleted : IEvent
    {
        public Guid UserId { get; set; }
        public DateTime DeletedAt { get; set; }
        public override string ToString() => $"User '{UserId}' deleted.";
    }
}
