using LoyaltyCard.Core;
using LoyaltyCard.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;

namespace LoyaltyCard.DependencyInjection
{
    public static class Startup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IAggregateStore, AggregateStore>();
            services.AddTransient(x =>
            {
                return GetConnection().Result;
            });

            return services;
        }

        private async static Task<EventStore.ClientAPI.IEventStoreConnection> GetConnection()
        {
            var connection = EventStore.ClientAPI.EventStoreConnection.Create(
                new IPEndPoint(IPAddress.Loopback, 1113)
            );

            await connection
                .ConnectAsync()
                .ConfigureAwait(false);

            return connection;
        }
    }
}
