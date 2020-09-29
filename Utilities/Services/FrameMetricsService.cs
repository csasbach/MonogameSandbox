using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Utilities.Abstractions;

namespace Utilities.Services
{
    public class FrameMetricsService : ComponentService, IFrameMetricsService
    {
        private class FrameMetricsBatch : IFrameMetricSampleBatch
        {
            public float MeanFps => Samples.Average();
            public float MaxFps => Samples.Max();
            public float MinFps => Samples.Min();
            public Queue<float> Samples { get; }
            public long BatchStartTimestamp { get; }

            public FrameMetricsBatch(int batchSize)
            {
                BatchStartTimestamp = Stopwatch.GetTimestamp();
                Samples = new Queue<float>(batchSize);
            }
        }

        public float CurrentFps { get; private set; }
        public float MeanFps { get; private set; }
        public float MaxFps { get; private set; }
        public float MinFps { get; private set; }
        public float MeanFpsDelta { get; private set; }
        public float MaxFpsDelta { get; private set; }
        public float MinFpsDelta { get; private set; }
        public int BatchSize { get; set; } = 100;
        public TimeSpan BatchTtl { get; set; } = TimeSpan.FromMinutes(1);
        public SortedList<long, IFrameMetricSampleBatch> Batches { get; }

        public FrameMetricsService(Game game) : base(game, typeof(IFrameMetricsService))
        {
            Batches = new SortedList<long, IFrameMetricSampleBatch>();
        }

        public override void Update(GameTime gameTime)
        {
            CurrentFps = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;

            // in with the new
            if(!Batches.Any() || Batches.Last().Value.Samples.Count >= BatchSize)
            {
                var newBatch = new FrameMetricsBatch(BatchSize);
                Batches.Add(newBatch.BatchStartTimestamp, newBatch);
            }

            // out with the old
            var firstKey = Batches.First().Key;
            if (Stopwatch.GetTimestamp() - firstKey > BatchTtl.Ticks)
            {
                Batches.Remove(firstKey);
            }

            // collect latest sample
            Batches.Last().Value.Samples.Enqueue(CurrentFps);
            MeanFps = Batches.SelectMany(b => b.Value.Samples).Average();
            MaxFps = Batches.SelectMany(b => b.Value.Samples).Max();
            MinFps = Batches.SelectMany(b => b.Value.Samples).Min();
            MeanFpsDelta = Batches.Select(b => {
                if (b.Key == Batches.Last().Key) return 0;
                var next = Batches.Values.ElementAt(Batches.IndexOfKey(b.Key) + 1);
                return next.MeanFps - b.Value.MeanFps;
            }).Average();
            MaxFpsDelta = Batches.Select(b => {
                if (b.Key == Batches.Last().Key) return 0;
                var next = Batches.Values.ElementAt(Batches.IndexOfKey(b.Key) + 1);
                return next.MaxFps - b.Value.MaxFps;
            }).Average();
            MinFpsDelta = Batches.Select(b => {
                if (b.Key == Batches.Last().Key) return 0;
                var next = Batches.Values.ElementAt(Batches.IndexOfKey(b.Key) + 1);
                return next.MinFps - b.Value.MinFps;
            }).Average();

            base.Update(gameTime);
        }
    }
}
