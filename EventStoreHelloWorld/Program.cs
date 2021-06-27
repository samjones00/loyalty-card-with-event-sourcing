using EventStore.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using EventStoreHelloWorld.Domain;

namespace EventStoreHelloWorld
{
    class Program
    {
        private static EventStoreClient _client;
  
        static async Task Main(string[] args)
        {
            Console.WriteLine("Connecting to event store...");
            var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=true");
            _client = new EventStoreClient(settings);

            var userId = Guid.Parse("2bdc5526-c9ce-4027-b4b0-976b11707d12");
    
            var streamName = GetStreamName<User>(userId);

            var handler = new UserService(await GetConnection(), streamName);

            await handler.Handle(new UserContracts.Create
            {
                UserId = userId
            });

            await handler.Handle(new UserContracts.ChangeName
            {
                UserId = userId,
                FirstName = "Sam",
                LastName = "Jones"
            });

            await handler.Handle(new UserContracts.ChangeName
            {
                UserId = userId,
                FirstName = "Samuel",
                LastName = "Jones"
            });
        }

        private static string GetStreamName<T>(Guid aggregateId) => $"{typeof(T).FullName}-{aggregateId}";

        public static async Task SaveEvent<T>(string streamName, T message) where T : class
        {
            var cancellationToken = new CancellationToken();

            var eventData = new EventData(
                eventId: Uuid.NewUuid(),
                type: message.GetType().Name,
                data: JsonSerializer.SerializeToUtf8Bytes(message)
            );

            await _client.AppendToStreamAsync(
               streamName: streamName,
                expectedState: StreamState.Any,
                eventData: new[] { eventData },
                cancellationToken: cancellationToken
            );
        }

        public static async Task<IEnumerable<IEvent>> ReadEvents(string streamName)
        {
            var cancellationToken = new CancellationToken();

            EventStoreClient.ReadStreamResult events = _client.ReadStreamAsync(
                direction: Direction.Forwards,
                streamName: streamName,
                revision: StreamPosition.Start,
                cancellationToken: cancellationToken);

            var list = new List<IEvent>();

            await foreach (var @event in events)
            {
                var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());
                Console.WriteLine(json);

                var o = DeserializeEvent(json, @event.Event.EventType);
                list.Add(o);
            }

            return list;
        }

        static async Task<EventStore.ClientAPI.IEventStoreConnection> GetConnection()
        {
            var connection = EventStore.ClientAPI.EventStoreConnection.Create(
                new IPEndPoint(IPAddress.Loopback, 1113)
            );

            await connection
                .ConnectAsync()
                .ConfigureAwait(false);

            return connection;
        }


        static IEvent DeserializeEvent(string json, string typeName)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).First(x => x.AssemblyQualifiedName == typeName);
            var result = (IEvent)JsonSerializer.Deserialize(json, type);

            return result;
        }
    }
}
