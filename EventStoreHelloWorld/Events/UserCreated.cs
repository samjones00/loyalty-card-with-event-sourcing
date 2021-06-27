using System;

namespace EventStoreHelloWorld.Events
{
    public class UserCreated : IEvent
    {
        public Guid UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
