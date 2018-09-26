using System;

namespace AppInsights.EventHub.Sink.NetFramework.Telemetry.Configuration
{
    public class EventHubSinkSettings
    {
        private BatchBufferConfig _batchBufferSettings;

        public EventHubSinkSettings()
        {
            _batchBufferSettings = new BatchBufferConfig();
        }

        /// <summary>
        /// The full URI to your event hub - should be int he form https://your-eventhub-namespace.servicebus.windows.net/event-hub-name
        /// </summary>
        public Uri EventHubUri { get; set; }

        /// <summary>
        /// The access key value associated with the <see cref="KeyName"/> you have created on the EventHub
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// The name of the access key used for the SAS token
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// Configuration settings for batching items to send to the event hub
        /// </summary>
        public BatchBufferConfig BatchBufferSettings
        {
            get => _batchBufferSettings;

            set => _batchBufferSettings = value ?? throw new ArgumentNullException();
        }
    }
}