using System;
using System.Threading.Tasks;
using LoyaltyCard.Core.Models;
using LoyaltyCard.Domain.Contracts.Customer;
using LoyaltyCard.Domain.Interfaces;

namespace LoyaltyCard.Core
{
    public class CustomerService: ICustomerService
    {
        private readonly IAggregateStore _aggregateStore;

        public CustomerService(IAggregateStore aggregateStore)
        {
            _aggregateStore = aggregateStore;
        }

        public Task Handle<T>(T command) where T : class
        {
            switch (command)
            {
                case CreateCustomer x:
                    return Execute(x.CustomerId, user => user.CreateUser(x.CustomerId));
                case ChangeCustomerName x:
                    return Execute(x.CustomerId, user => user.ChangeName(x.FirstName, x.LastName));
                case Delete x:
                    return Execute(x.CustomerId, user => user.Delete());
                default:
                    return Task.CompletedTask;
            }
        }

        async Task Execute(Guid id, Func<User, Task> update)
        {
            var ad = await _aggregateStore.Load<User>(id);
            await update(ad);
            await _aggregateStore.Save(ad);
        }

        Task Execute(Guid id, Action<User> update) => Execute(id, user => { update(user); return Task.CompletedTask; });
    }
}
