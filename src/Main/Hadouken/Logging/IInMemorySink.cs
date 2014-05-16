using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Hadouken.Logging
{
    public interface IInMemorySink : ILogEventSink
    {
        IEnumerable<LogEvent> LogEvents { get; }
    }
}
