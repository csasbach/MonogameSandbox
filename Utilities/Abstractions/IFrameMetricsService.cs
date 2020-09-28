using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Abstractions
{
    public interface IFrameMetricsService
    {
        float CurrentFps { get; }
        float MeanFps { get; }
        float MaxFps {get; }
        float MinFps { get; }
        float MeanFpsDelta { get; }
        float MaxFpsDelta { get; }
        float MinFpsDelta { get; }
        int BatchSize { get; set; }
        TimeSpan BatchTtl { get; set; }
        SortedList<long, IFrameMetricSampleBatch> Batches { get; }
    }

    public interface IFrameMetricSampleBatch
    {
        float MeanFps { get; }
        float MaxFps { get; }
        float MinFps { get; }
        Queue<float> Samples { get; }
        long BatchStartTimestamp { get; }
    }
}
