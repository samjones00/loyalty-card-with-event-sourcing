using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoyaltyCard.Core.Services
{
    public class WebhookService
    {
        public void Handle(string body)
        {
            var jsonArray = JArray.Parse(body);
            var request = JObject.Parse(jsonArray[0].ToString());

            var type = (string)request["eventType"];

            switch (type)
            {
                case "Microsoft.EventGrid.SubscriptionValidationEvent":
                    // validate
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
