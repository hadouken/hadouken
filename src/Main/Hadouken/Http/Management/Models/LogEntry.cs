using System;

namespace Hadouken.Http.Management.Models
{
    public class LogEntry
    {
        public DateTimeOffset Timestamp { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }
    }
}
