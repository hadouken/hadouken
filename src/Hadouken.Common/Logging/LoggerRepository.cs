using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Hadouken.Common.Logging
{
    public sealed class LoggerRepository : ILoggerRepository, ILogEventSink
    {
        private readonly IList<LogEntry> _entries = new List<LogEntry>();

        public IEnumerable<LogEntry> GetAll()
        {
            return new List<LogEntry>(_entries);
        }

        public void Emit(LogEvent logEvent)
        {
            var level = TranslateLogLevel(logEvent.Level);
            var source = string.Empty;

            if (logEvent.Properties.ContainsKey("SourceContext"))
            {
                var ctx = logEvent.Properties["SourceContext"] as ScalarValue;
                source = ctx != null ? ctx.Value.ToString() : string.Empty;
            }

            _entries.Add(new LogEntry
            {
                Level = level,
                Message = logEvent.RenderMessage(),
                Source = source,
                Timestamp = logEvent.Timestamp
            });
        }

        private LogLevel TranslateLogLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug:
                    return LogLevel.Debug;
                case LogEventLevel.Error:
                    return LogLevel.Error;
                case LogEventLevel.Fatal:
                    return LogLevel.Fatal;
                case LogEventLevel.Information:
                    return LogLevel.Info;
                case LogEventLevel.Verbose:
                    return LogLevel.Trace;
                case LogEventLevel.Warning:
                    return LogLevel.Warn;
            }

            throw new NotImplementedException();
        }
    }
}