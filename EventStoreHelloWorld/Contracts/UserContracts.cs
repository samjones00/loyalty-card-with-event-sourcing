using System;

namespace EventStoreHelloWorld.UserContracts
{
    public class Create
    {
        public Guid UserId { get; set; }
        public override string ToString() => $"Creating user '{UserId}'...";
    }

    public class ChangeName
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string ToString() => $"Changing name for user '{UserId}'...";
    }

    public class Delete
    {
        public Guid UserId { get; set; }    
        public override string ToString() => $"Deleting user '{UserId}'...";
    }
}
