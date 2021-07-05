using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using LoyaltyCard.Domain.Interfaces;
using LoyaltyCard.Providers.EventStore.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoyaltyCard.Providers.EventStore
{
    public class AggregateStore : IAggregateStore
    {
        const int MaxReadSize = 4096;
        readonly IEventStoreConnection _connection;
        readonly UserCredentials _userCredentials;

        public AggregateStore(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public async Task<T> Load<T>(Guid userId, CancellationToken cancellationToken = default)
            where T : IAggregate, new()
        {
            var streamName = StreamExtensions.GetStreamName<T>(userId);
            //if (IsNullOrWhiteSpace(aggregateId))
            //    throw new ArgumentException("Value cannot be null or whitespace.", nameof(aggregateId));

            var aggregate = new T();

            var nextPageStart = 0L;
            do
            {
                var page = await _connection.ReadStreamEventsForwardAsync(
                    streamName, nextPageStart, MaxReadSize, false, _userCredentials);

                if (page.Events.Length > 0)
                {
                    aggregate.Load(
                        page.Events.Last().Event.EventNumber,
                        Enumerable.ToArray<object>(page.Events.Select(re => Deserialize(re.Event.Data, re.Event.EventType))));
                }

                nextPageStart = !page.IsEndOfStream ? page.NextEventNumber : -1;
            } while (nextPageStart != -1);

            // Log.Debug("Loaded {aggregate} changes from stream {stream}", aggregate, _stream);

            return aggregate;
        }


        /// <summary>
        ///     Saves changes to the store.
        /// </summary>
        public async Task<(long NextExpectedVersion, long LogPosition, long CommitPosition)> Save<T>(
            T aggregate, CancellationToken cancellationToken = default)
            where T : IAggregate
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var streamName = StreamExtensions.GetStreamName<T>(aggregate.Id);

            var changes = aggregate.GetChanges()
                .Select(e => new EventData(Guid.NewGuid(), e.GetType().FullName, true, Serialize(e), null)).ToArray();

            if (!changes.Any())
            {
                //Log.Information("{Id} v{Version} aggregate has no changes.", aggregate.Id, aggregate.Version);
                return default;
            }

            //var stream = _getStreamName(typeof(T), aggregate.Id.ToString());

            WriteResult result;
            try
            {
                result = await _connection.AppendToStreamAsync(streamName, aggregate.Version, changes, _userCredentials);
            }
            catch (Exception)
            {
                var page = await _connection.ReadStreamEventsBackwardAsync(streamName, -1, 1, false, _userCredentials);
                throw new Exception(
                    $"Failed to append stream {streamName} with expected version {aggregate.Version}. " +
                    $"{(page.Status == SliceReadStatus.StreamNotFound ? "Stream not found!" : $"Current Version: {page.LastEventNumber}")}");
            }

            //Log.Debug("Saved {count} {aggregate} change(s) into stream {streamName}", changes.Length, aggregate, stream);

            //foreach (var change in aggregate.GetChanges())
            // Log.Information(change.ToString());

            return (
                result.NextExpectedVersion,
                result.LogPosition.CommitPosition,
                result.LogPosition.PreparePosition);
        }

        public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        public byte[] Serialize(object obj) =>
           Encoding.UTF8.GetBytes((string) JsonConvert.SerializeObject(obj, DefaultSettings));

        public object Deserialize(byte[] data, string typeName)
        {
            var type = GetType(typeName);
            var result = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), type, DefaultSettings);

            return result;
        }

        public Type GetType(string typeName) => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).First(x => x.FullName == typeName);
    }
}
