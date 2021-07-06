using LoyaltyCard.Domain.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace LoyaltyCard.Providers.Cosmos.DependencyInjection
{
    public static class Startup
    {
        public static IServiceCollection RegisterCosmosProvider(this IServiceCollection services)
        {
            services.AddTransient<IAggregateStore, AggregateStore>();
            services.AddTransient<CosmosClient>(x => GetConnection());

            return services;
        }

        private static CosmosClient GetConnection()
        {
            var connectionString = "";

            var client = new CosmosClient(connectionString, new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    IgnoreNullValues = true
                }
            });

            return client;
        }
    }
}
