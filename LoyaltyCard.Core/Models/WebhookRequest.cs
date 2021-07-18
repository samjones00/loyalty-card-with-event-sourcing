using LoyaltyCard.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace LoyaltyCard.Core.Services
{
    public class WebhookRequest
    {
        public string EventType { get; set; }
        public string ValidationUrl { get; set; }
        public string ValidationCode { get; set; }


        public WebhookRequest(string body)
        {
            JArray jsonArray = JArray.Parse(body);

            var obj = JObject.Parse(jsonArray[0].ToString());

            EventType = (string)obj[nameof(EventType).ToCamelCase()];

            ValidationUrl = (string)obj["data"][nameof(ValidationUrl).ToCamelCase()];
            ValidationCode = (string)obj["data"][nameof(ValidationCode).ToCamelCase()];
        }
    }
}

