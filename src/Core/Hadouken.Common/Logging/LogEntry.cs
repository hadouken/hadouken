using System;

namespace Hadouken.Common.Logging {
    public sealed class LogEntry {
        public DateTimeOffset Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string ExceptionString { get; set; }
    }
}