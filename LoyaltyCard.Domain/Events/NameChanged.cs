using System;
using LoyaltyCard.Domain.Interfaces;

namespace LoyaltyCard.Domain.Events
{
    public class NameChanged : IEvent
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
