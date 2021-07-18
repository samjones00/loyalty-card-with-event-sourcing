using LoyaltyCard.Domain.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace LoyaltyCard.Providers.Cosmos.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCosmosProvider(this IServiceCollection services)
        {
            services.AddTransient<IAggregateStore, AggregateStore>();
            services.AddTransient<CosmosClient>(x => GetConnection());

            return services;
        }

        private static CosmosClient GetConnection()
        {
            var connectionString =
                "AccountEndpoint=https://cosmos-sjo.documents.azure.com:443/;AccountKey=u1kbzfCfnqr8Biq57WTqqsB1GDKqK4RAiN3JU6Ttq6AVpl67WsYkvRtbweNv0AjvZNeFZ6sM2F6QWhMPgXuKkw==;";

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
