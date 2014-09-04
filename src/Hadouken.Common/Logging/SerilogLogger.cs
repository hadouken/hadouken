using System;
using Serilog.Events;

namespace Hadouken.Common.Logging
{
    public sealed class SerilogLogger : ILogger
    {
        private static readonly object _lock = new object();
        private readonly Serilog.ILogger _logger;

        public SerilogLogger(Serilog.ILogger logger, Type source)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (source == null) throw new ArgumentNullException("source");

            _logger = logger.ForContext(source);
        }

        public void Log(LogLevel logLevel, string message, params object[] propertyValues)
        {
            var level = TranslateLogLevel(logLevel);

            lock (_lock)
            {
                _logger.Write(level, message, propertyValues);
            }
        }

        public void Log(LogLevel logLevel, Exception exception, string message, params object[] propertyValues)
        {
            var level = TranslateLogLevel(logLevel);

            lock (_lock)
            {
                _logger.Write(level, exception, message, propertyValues);
            }
        }

        private LogEventLevel TranslateLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Fatal:
                    return LogEventLevel.Fatal;
                case LogLevel.Info:
                    return LogEventLevel.Information;
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;
                case LogLevel.Warn:
                    return LogEventLevel.Warning;
            }

            throw new NotImplementedException();
        }
    }
}
