using System;

namespace AppInsights.EventHub.Sink.NetCore.Telemetry.Configuration
{
    public class BatchBufferConfig
    {
        private const int MaxBacklogDefault = 1000;
        private const int MaxWindowCountDefault = 1000;
        private const int WindowSizeDefault = 10;

        public BatchBufferConfig()
        {
            MaxBacklog = MaxBacklogDefault;
            MaxWindowCount = MaxWindowCountDefault;
            BatchFlushInterval = TimeSpan.FromSeconds(WindowSizeDefault);
        }

        public int MaxBacklog { get; set; }

        public int MaxWindowCount { get; set; }

        public TimeSpan BatchFlushInterval { get; set; }
    }
}