using LoyaltyCard.Domain.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Text;

namespace LoyaltyCard.Core
{
    public class Serializer : ISerializer
    {
        public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        public byte[] SerializeToByteArray(object obj) =>
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, DefaultSettings));

        public object DeserializeFromByteArray(byte[] data, string typeName)
        {
            var type = GetType(typeName);
            var result = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), type, DefaultSettings);

            return result;
        }

        public string SerializeToString(object obj) => JsonConvert.SerializeObject(obj, DefaultSettings);

        public object DeserializeFromString(string json, string typeName)
        {
            var type = GetType(typeName);

            return JsonConvert.DeserializeObject(json, type);
        }

        private Type GetType(string typeName) => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).First(x => x.FullName == typeName);
    }
}
