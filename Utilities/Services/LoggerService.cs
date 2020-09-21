using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Services
{
    public class LoggerService : ServiceBase, ILogger
    {
        private class Scope<TState> : IDisposable
        {
            public TState State { get; }
            public Func<TState, string> Formatter { get; set; }

            public Scope(TState state, Func<TState, string> formatter)
            {
                State = state;
                Formatter = formatter;
            }

            public override string ToString()
            {
                return Formatter(State);
            }

            #region Dispose Pattern

            private bool disposedValue;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~Scope()
            // {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            #endregion Dispose Pattern
        }

        private LogLevel _logLevel = LogLevel.Warning;
        private readonly List<ILogQueue> _logQueues = new List<ILogQueue>();
        private Func<object, string> _formatter = d => d.ToString();

        public LoggerService(Game game) : base(game, typeof(LoggerService)) { }        

        public IDisposable BeginScope<TState>(TState state)
        {
            return new Scope<TState>(state, _formatter as Func<TState, string>);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var log = $"[{logLevel}]-[{eventId.Name}]-[{eventId.Id}] {formatter(state, exception)}";
            foreach (var logQueue in _logQueues) logQueue.EnqueueAsync(log);
        }

        public void SetLogLevel(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        public void AddLogQueue(ILogQueue logQueue)
        {
            _logQueues.Add(logQueue);
        }

        public void SetStateFormatter(Func<object, string> formatter)
        {
            _formatter = formatter;
        }
    }

    public static class LoggerServiceExtensions
    {
        public static void LogCritical(this LoggerService logger, IDisposable scope, string codeLocation, string message, Exception exception)
        {
            LogCritical(logger, scope, codeLocation, 0, message, exception);
        }

        public static void LogCritical(this LoggerService logger, IDisposable scope, string codeLocation, int iteration, string message, Exception exception)
        {
            Log(logger, LogLevel.Critical, scope, codeLocation, iteration, message, exception);
        }

        public static void LogError(this LoggerService logger, IDisposable scope, string codeLocation, string message, Exception exception)
        {
            LogError(logger, scope, codeLocation, 0, message, exception);
        }

        public static void LogError(this LoggerService logger, IDisposable scope, string codeLocation, int iteration, string message, Exception exception)
        {
            Log(logger, LogLevel.Error, scope, codeLocation, iteration, message, exception);
        }

        public static void LogWarning(this LoggerService logger, IDisposable scope, string codeLocation, string message, Exception exception)
        {
            LogWarning(logger, scope, codeLocation, 0, message, exception);
        }

        public static void LogWarning(this LoggerService logger, IDisposable scope, string codeLocation, int iteration, string message, Exception exception)
        {
            Log(logger, LogLevel.Warning, scope, codeLocation, iteration, message, exception);
        }

        public static void LogInfo(this LoggerService logger, IDisposable scope, string codeLocation, string message, Exception exception)
        {
            LogInfo(logger, scope, codeLocation, 0, message, exception);
        }

        public static void LogInfo(this LoggerService logger, IDisposable scope, string codeLocation, int iteration, string message, Exception exception)
        {
            Log(logger, LogLevel.Information, scope, codeLocation, iteration, message, exception);
        }

        public static void LogDebug(this LoggerService logger, IDisposable scope, string codeLocation, string message, Exception exception)
        {
            LogDebug(logger, scope, codeLocation, 0, message, exception);
        }

        public static void LogDebug(this LoggerService logger, IDisposable scope, string codeLocation, int iteration, string message, Exception exception)
        {
            Log(logger, LogLevel.Debug, scope, codeLocation, iteration, message, exception);
        }

        public static void LogTrace(this LoggerService logger, IDisposable scope, string codeLocation, string message, Exception exception)
        {
            LogTrace(logger, scope, codeLocation, 0, message, exception);
        }

        public static void LogTrace(this LoggerService logger, IDisposable scope, string codeLocation, int iteration, string message, Exception exception)
        {
            Log(logger, LogLevel.Trace, scope, codeLocation, iteration, message, exception);
        }

        public static void Log(this LoggerService logger, LogLevel logLevel, IDisposable scope, string codeLocation, string message, Exception exception)
        {
            Log(logger, logLevel, scope, codeLocation, 0, message, exception);
        }

        public static void Log(this LoggerService logger, LogLevel logLevel, IDisposable scope, string codeLocation, int iteration, string message, Exception exception)
        {
            logger.Log(logLevel, new EventId(iteration, codeLocation), scope, exception, (s, e) => $"{s} - {message}{(e is null ? string.Empty : $" - {e.Message}")}");
        }

    }

    public interface ILogQueue
    {
        Task EnqueueAsync(string log);
    }

    public interface ILogWriter
    {
        void Write(string log);
    }

    public abstract class LogQueueBase : ILogQueue
    {
        private readonly int _maxQueueSize = 1000;
        private readonly BlockingCollection<Task> _writeQueue;
        private readonly ILogWriter _logWriter;

        /// <summary>
        /// The approximate amount of time to continue trying to
        /// enqueue a task to the log queue.
        /// 
        /// The actual time is this timespan rounded up to the nearest
        /// power of 2 in milliseconds.
        /// </summary>
        public TimeSpan MaxEnqueueRetryWait { get; set; } = TimeSpan.FromSeconds(1);

        protected LogQueueBase(ILogWriter logWriter) : this((int)TimeSpan.FromSeconds(1).TotalMilliseconds, logWriter) { }

        protected LogQueueBase(int maxQueueSize, ILogWriter logWriter)
        {
            _maxQueueSize = maxQueueSize;
            _writeQueue = new BlockingCollection<Task>(_maxQueueSize);
            _logWriter = logWriter;
            Dequeue();
        }

        public async Task EnqueueAsync(string log)
        {
            await Task.Run(() =>
            {
                var writeTask = new Task(() => _logWriter.Write(log));
                var sleepMilliseconds = 0;
                var maxTime = RoundUpToPowerOfTwo((int)MaxEnqueueRetryWait.TotalMilliseconds);
                while (sleepMilliseconds < maxTime && !_writeQueue.TryAdd(writeTask))
                {
                    if (sleepMilliseconds < 2)
                        sleepMilliseconds = 2;
                    else
                        sleepMilliseconds *= 2;

                    Thread.Sleep(sleepMilliseconds);
                }
            }).ConfigureAwait(false);
        }

        protected async void Dequeue()
        {
            await Task.Run(() =>
            {
                var sleepMilliseconds = 0;
                var maxTime = RoundUpToPowerOfTwo((int)MaxEnqueueRetryWait.TotalMilliseconds);
                Task task = null;
                while (sleepMilliseconds < maxTime && !_writeQueue.TryTake(out task))
                {
                    if (sleepMilliseconds < 2)
                        sleepMilliseconds = 2;
                    else
                        sleepMilliseconds *= 2;

                    Thread.Sleep(sleepMilliseconds);
                }
                task?.RunSynchronously();
            }).ConfigureAwait(false);

            Dequeue();
        }

        private static int RoundUpToPowerOfTwo(int n)
        {
            int count = 0;

            if (n > 0 && (n & (n - 1)) == 0)
                return n;

            while (n != 0)
            {
                n >>= 1;
                count += 1;
            }

            return 1 << count;
        }
    }

    public class DebugLogWriter : ILogWriter
    {
        public void Write(string log)
        {
            Debug.WriteLine(log);
        }
    }

    public class DebugLogQueue : LogQueueBase
    {
        public DebugLogQueue() : this((int)TimeSpan.FromSeconds(1).TotalMilliseconds) { }
        public DebugLogQueue(int maxQueueSize) : base(maxQueueSize, new DebugLogWriter()) { }
    }
}
