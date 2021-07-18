using FluentAssertions;
using LoyaltyCard.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoyaltyCard.Core.Tests.Models
{
    public class WebhookRequestTests
    {
        [Test]
        public void ShouldParse()
        {
            var json = GetContents();

            var request = new WebhookRequest(json);

            request.ValidationCode.Should().Be("D9192B09-82EA-44D6-8580-B4E4633F1D2E");
            request.ValidationUrl.Should().Be("https://rp-uksouth.eventgrid.azure.net:553/eventsubscriptions/webhook/validate?id=D9192B09-82EA-44D6-8580-B4E4633F1D2E&t=2021-07-18T19:19:01.2177564Z&apiVersion=2020-10-15-preview&token=H2S4f%2bp5QVubOOBf2YoeuI56iKkiOLedXxTlZEkZOkg%3d");
            request.EventType.Should().Be("Microsoft.EventGrid.SubscriptionValidationEvent");
        }

        public static string GetContents()
        {
            var filename = "SubscriptionValidationEvent.json";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", filename);
            var json = File.ReadAllText(filePath);
            return json;
        }
    }
}
