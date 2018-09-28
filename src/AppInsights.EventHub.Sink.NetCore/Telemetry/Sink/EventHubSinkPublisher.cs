using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppInsights.EventHub.Sink.NetCore.Telemetry.Batching;
using AppInsights.EventHub.Sink.NetCore.Telemetry.Configuration;
using Microsoft.ApplicationInsights.Channel;

namespace AppInsights.EventHub.Sink.NetCore.Telemetry.Sink
{
    public abstract class EventHubSinkPublisher : BatchingPublisher
    {
        private readonly EventHubSinkSettings _eventHubSettings;

        protected EventHubSinkPublisher(EventHubSinkSettings eventHubSettings)
            : base(eventHubSettings.BatchBufferSettings)
        {
            _eventHubSettings = eventHubSettings;
        }

        protected override async Task Publish(ICollection<ITelemetry> telemetryEvents)
        {
            if (telemetryEvents == null)
                return;

            try
            {
                await PublishToEventHub(telemetryEvents);
            }
            catch (Exception e)
            {
                // do exception logging
            }
        }
        

        protected override bool Filter(ITelemetry item)
        {
            if (item is DependencyTelemetry dependencyData)
            {
                // Filter out dependency telemetry generated as part of this Sink
                if (dependencyData.Target.Contains(_eventHubSettings.EventHubUri.Host))
                {
                    return true;
                }
            }

            return base.Filter(item);
        }        

        protected async Task PublishToEventHub(ICollection<ITelemetry> events)
        {
            try
            {
                var transmission = new EventHubTransmission(_eventHubSettings, events);

                await transmission.SendAsync();
            }
            catch (Exception e)
            {
                // do some logging
            }
        }
    }
}
