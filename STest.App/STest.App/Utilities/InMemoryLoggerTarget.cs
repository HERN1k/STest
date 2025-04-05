using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using NLog;
using NLog.Targets;

namespace STest.App.Utilities
{
    /// <summary>
    /// In-memory logger target
    /// </summary>
    [Target("InMemoryLogger")]
    public sealed partial class InMemoryLoggerTarget : TargetWithLayout
    {
        /// <summary>
        /// Maximum number of logs to keep in memory
        /// </summary>
        public int MaxLogs { get; set; } = 100;
        /// <summary>
        /// Last logs
        /// </summary>
        public IEnumerable<LogEventInfo> Logs
        {
            get => m_logs.ToArray();
        }
        /// <summary>
        /// Event raised when a log is received
        /// </summary>
        public event EventHandler<LogEventInfo>? LogReceived;

        /// <summary>
        /// Logs queue
        /// </summary>
        private readonly ConcurrentQueue<LogEventInfo> m_logs = new();

        protected override void Write(LogEventInfo log)
        {
            m_logs.Enqueue(log);

            if (m_logs.Count >= 100)
            {
                m_logs.TryDequeue(out _);
            }

            LogReceived?.Invoke(this, log);
        }

        /// <summary>
        /// Clear logs
        /// </summary>
        public void Clear() => m_logs.Clear();
    }
}