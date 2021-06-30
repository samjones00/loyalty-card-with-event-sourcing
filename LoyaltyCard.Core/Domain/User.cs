using LoyaltyCard.Core.Events;
using System;

namespace LoyaltyCard.Core.Domain
{
    public class User : Aggregate
    {
        public string FirstName { get; set;  }
        public string LastName { get; set; }
        bool WasRemoved { get; set; }

        protected override void When(object e)
        {
            switch (e)
            {
                case UserCreated x:
                    Id = x.UserId;
                    break;
                case NameChanged x:
                    FirstName = x.FirstName;
                    LastName = x.LastName;
                    break;
                case UserDeleted _:
                    WasRemoved = true;
                    break;
            }
        }

        public void CreateUser(Guid id)
        {
            if (Version >= 0)
                throw new Exception("Already registered.");
             
            Apply(new UserCreated
            {
                UserId = id,
                DateCreated = DateTime.UtcNow
            });
        }

        public void ChangeName(string firstName, string lastName)
        {
            if (Version == -1)
                throw new Exception("Not found.");

            if (WasRemoved)
                throw new Exception();

            if (firstName == FirstName && lastName == LastName) return;

            Apply(new NameChanged
            {
                UserId = Id,
                FirstName = firstName,
                LastName = lastName,
                ChangedAt = DateTime.UtcNow
            });
        }

        public void Delete()
        {
            if (Version == -1)
                throw new Exception("not found.");

            if (WasRemoved) return;

            Apply(new UserDeleted
            {
                UserId = Id,
                DeletedAt = DateTime.UtcNow
            });
        }
    }
}
