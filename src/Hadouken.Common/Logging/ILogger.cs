using System;

namespace Hadouken.Common.Logging
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, string message, Exception exception = null);
    }
}
