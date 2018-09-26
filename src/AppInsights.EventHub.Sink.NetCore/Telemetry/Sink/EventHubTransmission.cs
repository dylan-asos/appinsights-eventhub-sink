using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using AppInsights.EventHub.Sink.NetCore.Telemetry.Configuration;
using AppInsights.EventHub.Sink.NetCore.Telemetry.Security;
using Microsoft.ApplicationInsights.Channel;

namespace AppInsights.EventHub.Sink.NetCore.Telemetry.Sink
{
    public class EventHubTransmission : Transmission
    {
        private readonly string _sharedAccessSignature;

        public EventHubTransmission(EventHubSinkSettings settings, ICollection<ITelemetry> telemetryItems)
            : base(new Uri(settings.EventHubUri + "/messages"), telemetryItems)
        {
            _sharedAccessSignature = SasKeyGenerator.CreateSasToken(settings.EventHubUri.ToString(), settings.KeyName, settings.AccessKey);
        }

        protected override HttpRequestMessage CreateRequestMessage(Uri address, Stream contentStream)
        {
            var request = base.CreateRequestMessage(address, contentStream);

            request.Headers.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _sharedAccessSignature);

            return request;
        }
    }
}