using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using LoyaltyCard.Domain.Interfaces;

namespace LoyaltyCard.Providers.EventStore.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddEventStoreProvider(this IServiceCollection services)
        {
            services.AddTransient<IAggregateStore, AggregateStore>();
            services.AddTransient<IEventStoreConnection>(x => GetConnection().Result);

            return services;
        }

        private static async Task<IEventStoreConnection> GetConnection()
        {
            var connection = EventStoreConnection.Create(
                new IPEndPoint(IPAddress.Loopback, 1113)
            );

            await connection
                .ConnectAsync()
                .ConfigureAwait(false);

            return connection;
        }
    }
}
