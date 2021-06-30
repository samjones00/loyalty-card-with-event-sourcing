using System;

namespace LoyaltyCard.Core.Events
{
    public class UserDeleted : IEvent
    {
        public Guid UserId { get; set; }
        public DateTime DeletedAt { get; set; }
        public override string ToString() => $"User '{UserId}' deleted.";
    }
}
