using System;

namespace LoyaltyCard.Domain.Contracts.Customer
{
    public class CreateCustomer
    {
        public Guid CustomerId { get; set; }
        public override string ToString() => $"Creating user '{CustomerId}'...";
    }

    public class ChangeCustomerName
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string ToString() => $"Changing name for customer '{CustomerId}'...";
    }

    public class Delete
    {
        public Guid CustomerId { get; set; }    
        public override string ToString() => $"Deleting customer '{CustomerId}'...";
    }
}
