using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace QuickStartJsonFormat
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Connecting to event store...");

            await JsonFormat();
        }

        public static async Task JsonFormat()
        {
            var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@eventstore:1113"));
            await conn.ConnectAsync();

            const string streamName = "test-stream";
            const string eventType = "testEvent";
            const string data = "{ \"a\":\"2\"}";
            const string metadata = "{}";

            var eventPayload = new EventData(
                eventId: Guid.NewGuid(),
                type: eventType,
                isJson: true,
                data: Encoding.UTF8.GetBytes(data),
                metadata: Encoding.UTF8.GetBytes(metadata)
            );
            var result = await conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventPayload);


            var readEvents = await conn.ReadStreamEventsForwardAsync(streamName, 0, 10, true);

            foreach (var evt in readEvents.Events)
            {
                Console.WriteLine(Encoding.UTF8.GetString(evt.Event.Data));
            }
        }
    }
}
