using System;
using LoyaltyCard.Domain.Interfaces;

namespace LoyaltyCard.Domain.Events
{
    public class UserCreated : IEvent
    {
        public Guid UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
