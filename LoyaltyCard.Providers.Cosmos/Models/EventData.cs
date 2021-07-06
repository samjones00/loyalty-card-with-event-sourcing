using Newtonsoft.Json;

namespace LoyaltyCard.Providers.Cosmos.Models
{
    public class EventData
    {
        public EventData(string id, string aggregateId, string eventType, bool isJson, string data, string metadata)
        {
            Id = id;
            AggregateId = aggregateId;
            EventType = eventType;
            IsJson = isJson;
            Data = data;
            Metadata = metadata;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        public string AggregateId { get; set; }

        public bool IsJson { get; set; }

        public string EventType { get; set; }

        public string Metadata { get; set; }

        public string Data { get; set; }

        public long Version { get; set; }
    }
}
