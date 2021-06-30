using System;

namespace LoyaltyCard.Core.Events
{
    public class UserCreated : IEvent
    {
        public Guid UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
