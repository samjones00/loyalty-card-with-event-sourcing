using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LoyaltyCard.Domain.Interfaces;
using LoyaltyCard.Providers.Cosmos.Extensions;
using LoyaltyCard.Providers.Cosmos.Models;
using Microsoft.Azure.Cosmos;

namespace LoyaltyCard.Providers.Cosmos
{
    public class AggregateStore : IAggregateStore
    {
        private readonly CosmosClient _client;
        private readonly ISerializer _serializer;

        public AggregateStore(CosmosClient client, ISerializer serializer)
        {
            _client = client;
            _serializer = serializer;
        }

        public async Task<T> Load<T>(Guid aggregateId, CancellationToken cancellationToken = default)
            where T : IAggregate, new()
        {
            var aggregate = new T();

            var containerName = CosmosExtensions.GetContainerName<T>();

            var db = await _client.CreateDatabaseIfNotExistsAsync("LoyaltyCardDB");
            var container = await db.Database.CreateContainerIfNotExistsAsync(containerName, "/AggregateId");

            QueryDefinition query = new QueryDefinition("select * from t where t.AggregateId = @aggregateId")
                .WithParameter("@aggregateId", aggregateId.ToString());

            FeedIterator<EventData> queryResultSetIterator = container.Container.GetItemQueryIterator<EventData>(query);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<EventData> currentResultSet = await queryResultSetIterator.ReadNextAsync();

                aggregate.Load(
                    currentResultSet.Last().Version,
                    Enumerable.ToArray(currentResultSet.Select(re =>
                        _serializer.DeserializeFromJson(re.Data, re.EventType))));
            }

            return aggregate;
        }

        public async Task<(long NextExpectedVersion, long LogPosition, long CommitPosition)> Save<T>(
            T aggregate, CancellationToken cancellationToken = default)
            where T : IAggregate
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var containerName = CosmosExtensions.GetContainerName<T>();

            var changes = aggregate.GetChanges()
                .Select(e => new EventData(Guid.NewGuid().ToString(), aggregate.Id.ToString(), e.GetType().FullName,
                    true, _serializer.SerializeToJson(e), null)).ToArray();

            for (int i = 0; i < changes.Length; i++)
            {
                changes[i].Version = aggregate.Version + i + 1;
            }

            if (!changes.Any())
            {
                //Log.Information("{Id} v{Version} aggregate has no changes.", aggregate.Id, aggregate.Version);
                return default;
            }

            var db = await _client.CreateDatabaseIfNotExistsAsync("LoyaltyCardDB");
            var container = await db.Database.CreateContainerIfNotExistsAsync(containerName, "/AggregateId");

            foreach (var change in changes)
            {
                await container.Container.UpsertItemAsync(change);
            }

            return (changes.Last().Version++, 0, 0);
        }
    }
}
