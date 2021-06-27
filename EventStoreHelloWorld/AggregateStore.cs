using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.String;

namespace EventStoreHelloWorld
{
    public class AggregateStore
    {
        const int MaxReadSize = 4096;
        readonly IEventStoreConnection _connection;
        readonly UserCredentials _userCredentials;
        private readonly string _stream;

        public AggregateStore(IEventStoreConnection connection, string stream)
        {
            _connection = connection;
            this._stream = stream;
        }

        public async Task<T> Load<T>(Guid userId, CancellationToken cancellationToken = default)
            where T : Aggregate, new()
        {
            //if (IsNullOrWhiteSpace(aggregateId))
            //    throw new ArgumentException("Value cannot be null or whitespace.", nameof(aggregateId));

            var aggregate = new T();

            var nextPageStart = 0L;
            do
            {
                var page = await _connection.ReadStreamEventsForwardAsync(
                    _stream, nextPageStart, MaxReadSize, false, _userCredentials);

                if (page.Events.Length > 0)
                {
                    aggregate.Load(
                        page.Events.Last().Event.EventNumber,
                        page.Events.Select(re => Deserialize(re.Event.Data, re.Event.EventType))
                        .ToArray());
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
            where T : Aggregate
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

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
                result = await _connection.AppendToStreamAsync(_stream, aggregate.Version, changes, _userCredentials);
            }
            catch (Exception)
            {
                var page = await _connection.ReadStreamEventsBackwardAsync(_stream, -1, 1, false, _userCredentials);
                throw new Exception(
                    $"Failed to append stream {_stream} with expected version {aggregate.Version}. " +
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
           Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, DefaultSettings));

        public object Deserialize(byte[] data, string typeName)
        {
            var type = GetType(typeName);
            var result = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), type, DefaultSettings);

            return result;
        }

        public Type GetType(string typeName) => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).First(x => x.FullName == typeName);
    }
}
