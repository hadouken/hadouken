using System.Collections.Generic;
using Serilog.Events;

namespace Hadouken.Logging
{
    public class InMemorySink : IInMemorySink
    {
        private readonly IList<LogEvent> _events = new List<LogEvent>();

        public void Emit(LogEvent logEvent)
        {
            _events.Add(logEvent);
        }

        public IEnumerable<LogEvent> LogEvents
        {
            get { return _events; }
        }
    }
}