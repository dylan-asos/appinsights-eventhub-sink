# appinsights-eventhub-sink
Custom appinsights pipeline that allows you to batch and send copies of telemetry to an EventHub

Source for both .NET Core and .NET Framework 4.6.1 - essentially the same, a few differences in initialisation and class EventHubTransmission

### Configuration

Create an **EventHubSinkSettings** instance and provide the relevant values

```csharp
var eventHubSinkSettings = new EventHubSinkSettings
{
    KeyName = "your-key-name",
    AccessKey = "your-access-key",                
    EventHubUri = new Uri("https://eventhub-namespace.servicebus.windows.net/event-hub-name"),
    BatchBufferSettings = new BatchBufferConfig()
    {
        MaxBacklog = 1000,
        MaxWindowCount = 1000,
        BatchFlushInterval = TimeSpan.FromSeconds(10)
    }                
};
```
##### KeyName
The name of the key you have created on the event hub. The key must have Send permissions

##### AccessKey
The access key value associated with the KeyName.

#####EventHubUri
The full URI to the event hub, in the format https://**eventhub-namespace**.servicebus.windows.net/**event-hub-name**

#####BatchBufferSettings
The settings to use when buffering events created by application insights, before they are transmitted to EventHub

#####MaxBacklog
Maximum amount of items in the buffer, when reached they will be flushed

#####MaxWindowCount
Maximum amount of items in any one batch

#####BatchFlushInterval
Timespan that represents how often the Flush will send data to Event Hubs.

### Add to Processor Chain

Add the **EventHubsSink** to the **TelemetryProcessorChainBuilder* for the active Telemetry configuration

```csharp
var config = TelemetryConfiguration.Active;

var builder = config.TelemetryProcessorChainBuilder;

builder.Use((next) => new EventHubsSink(eventHubSinkSettings, next)).Build();
```
