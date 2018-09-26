using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AppInsights.EventHub.Sink.NetFramework.Telemetry.Configuration;
using Microsoft.ApplicationInsights.Channel;

namespace AppInsights.EventHub.Sink.NetFramework.Telemetry.Batching
{
    public class BatchingPublisher : IDisposable
    {
        private readonly BatchBlock<ITelemetry> _batcher;
        private readonly BufferBlock<ITelemetry> _buffer;
        private readonly ActionBlock<ICollection<ITelemetry>> _publisher;

        protected readonly CancellationTokenSource TokenSource;

        protected BatchBufferConfig BatchBufferConfiguration;

        private readonly IDisposable[] _disposables;

        private readonly Timer _windowTimer;

        private int _disposeCount;
        private long _droppedEvents;
        private long _droppedEventsTotal;

        public BatchingPublisher(BatchBufferConfig config)
        {
            BatchBufferConfiguration = config ?? throw new ArgumentNullException(nameof(config));
            TokenSource = new CancellationTokenSource();

            _buffer = new BufferBlock<ITelemetry>(
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = BatchBufferConfiguration.MaxBacklog,
                    CancellationToken = TokenSource.Token
                });

            _batcher = new BatchBlock<ITelemetry>(
                BatchBufferConfiguration.MaxWindowCount,
                new GroupingDataflowBlockOptions
                {
                    BoundedCapacity = BatchBufferConfiguration.MaxWindowCount,
                    Greedy = true,
                    CancellationToken = TokenSource.Token
                });

            _publisher = new ActionBlock<ICollection<ITelemetry>>(
                async e => await Publish(e),
                new ExecutionDataflowBlockOptions
                {
                    // Maximum of one concurrent batch being published
                    MaxDegreeOfParallelism = 1,

                    // Maximum of three pending batches to be published
                    BoundedCapacity = 3,
                    CancellationToken = TokenSource.Token
                });

            _disposables = new[]
            {
                _buffer.LinkTo(_batcher),
                _batcher.LinkTo(_publisher)
            };

            _windowTimer = new Timer(Flush, null, BatchBufferConfiguration.BatchFlushInterval, BatchBufferConfiguration.BatchFlushInterval);
        }

        public virtual void Dispose()
        {
            if (Interlocked.Increment(ref _disposeCount) == 1)
            {
                _windowTimer?.Dispose();

                TokenSource.Cancel();
                _buffer.Completion.Wait();
                _batcher.Completion.Wait();
                _publisher.Completion.Wait();

                foreach (var d in _disposables)
                    d.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public virtual void PostEntry(ITelemetry item)
        {
            if (Filter(item)) return;

            if (!_buffer.Post(item))
                Interlocked.Increment(ref _droppedEvents);
        }

        protected virtual bool Filter(ITelemetry item)
        {
            return false;
        }

        protected virtual async Task Publish(ICollection<ITelemetry> telemetryEvents)
        {
            await Task.FromResult(true);
        }

        public void Flush(object state = null)
        {
            if (Interlocked.Read(ref _droppedEvents) != 0)
            {
                Interlocked.Add(ref _droppedEventsTotal, _droppedEvents);
            }

            Interlocked.Exchange(ref _droppedEvents, 0);
            _batcher?.TriggerBatch();
        }
    }
}