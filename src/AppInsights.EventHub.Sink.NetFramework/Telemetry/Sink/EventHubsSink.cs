﻿using AppInsights.EventHub.Sink.NetFramework.Telemetry.Configuration;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace AppInsights.EventHub.Sink.NetFramework.Telemetry.Sink
{
    public class EventHubsSink : EventHubSinkPublisher, ITelemetryProcessor
    {
	protected readonly ITelemetryProcessor Next;

	public EventHubsSink(EventHubSinkSettings settings, ITelemetryProcessor next) : base(settings)
	{
	    Next = next;
	}

	public void Process(ITelemetry item)
	{
	    PostEntry(item);
	    Next.Process(item);
	}        
    }
}
