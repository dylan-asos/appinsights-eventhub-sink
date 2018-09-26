using System;
using System.Collections.Generic;
using System.Net;
using AppInsights.EventHub.Sink.NetFramework.Telemetry.Configuration;
using AppInsights.EventHub.Sink.NetFramework.Telemetry.Security;
using Microsoft.ApplicationInsights.Channel;

namespace AppInsights.EventHub.Sink.NetFramework.Telemetry.Sink
{
    public class EventHubTransmission : Transmission
    {
        private readonly string _sharedAccessSignature;

        public EventHubTransmission(EventHubSinkSettings settings, ICollection<ITelemetry> telemetryItems)
            : base(new Uri(settings.EventHubUri + "/messages"), telemetryItems)
        {
            _sharedAccessSignature = SasKeyGenerator.CreateSasToken(settings.EventHubUri.ToString(), settings.KeyName, settings.AccessKey);
        }

        protected override WebRequest CreateRequest(Uri address)
        {
            var request = base.CreateRequest(address);

            request.Headers.Add("Authorization", _sharedAccessSignature);

            return request;
        }
    }
}