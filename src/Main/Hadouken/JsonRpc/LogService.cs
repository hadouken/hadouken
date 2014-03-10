using System;
using System.Collections.Generic;
using Hadouken.Fx.JsonRpc;
using NLog;

namespace Hadouken.JsonRpc
{
    public class LogService : IJsonRpcService
    {
        private readonly IDictionary<string, Logger> _loggers = new Dictionary<string, Logger>();
        private readonly object _loggersLock = new object();

        [JsonRpcMethod("core.logging.log")]
        public bool Log(string source, string logLevel, string message)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            var logger = GetLogger(source);
            var level = TranslateLogLevel(logLevel);

            logger.Log(level, message);

            return true;
        }

        private NLog.LogLevel TranslateLogLevel(string level)
        {
            var logLevel = NLog.LogLevel.FromString(level);

            if (logLevel != null)
            {
                return logLevel;
            }

            throw new ArgumentException("No suitable log level found: " + level, "level");
        }

        private Logger GetLogger(string source)
        {
            lock (_loggersLock)
            {
                if (!_loggers.ContainsKey(source))
                {
                    _loggers.Add(source, LogManager.GetLogger(source));
                }

                return _loggers[source];
            }
        }
    }

    public class LogEntry
    {
        public string Level { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }
    }

    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }
}
