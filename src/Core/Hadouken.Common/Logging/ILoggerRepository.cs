using System.Collections.Generic;

namespace Hadouken.Common.Logging
{
    public interface ILoggerRepository
    {
        IEnumerable<LogEntry> GetAll();
    }
}
