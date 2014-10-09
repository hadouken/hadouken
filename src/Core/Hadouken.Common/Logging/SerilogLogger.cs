using System;
using Serilog.Events;

namespace Hadouken.Common.Logging
{
    internal static class LogLock
    {
        public static readonly object Lock = new object();
    }

    public sealed class SerilogLogger<T> : ILogger<T>
    {
        private readonly Serilog.ILogger _logger;

        public SerilogLogger(Serilog.ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger.ForContext(typeof (T));
        }

        public void Log(LogLevel logLevel, string message, params object[] propertyValues)
        {
            var level = TranslateLogLevel(logLevel);

            lock (LogLock.Lock)
            {
                _logger.Write(level, message, propertyValues);
            }
        }

        public void Log(LogLevel logLevel, Exception exception, string message, params object[] propertyValues)
        {
            var level = TranslateLogLevel(logLevel);

            lock (LogLock.Lock)
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
