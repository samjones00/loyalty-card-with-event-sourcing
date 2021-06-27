using EventStore.ClientAPI;
using EventStoreHelloWorld.Domain;
using System;
using System.Threading.Tasks;

namespace EventStoreHelloWorld
{
    public class UserService
    {
        AggregateStore _store;

        public UserService(IEventStoreConnection connection, string streamName)
        {
            _store = new AggregateStore(connection, streamName);
        }

        public Task Handle<T>(T command) where T : class
        {
            switch (command)
            {
                case UserContracts.Create x:
                    return Execute(x.UserId, user => user.CreateUser(x.UserId));
                case UserContracts.ChangeName x:
                    return Execute(x.UserId, user => user.ChangeName(x.FirstName, x.LastName));
                case UserContracts.Delete x:
                    return Execute(x.UserId, user => user.Delete());
                default:
                    return Task.CompletedTask;
            }
        }

        async Task Execute(Guid id, Func<User, Task> update)
        {
            var ad = await _store.Load<User>(id);
            await update(ad);
            await _store.Save(ad);
        }

        Task Execute(Guid id, Action<User> update) => Execute(id, user => { update(user); return Task.CompletedTask; });
    }
}
