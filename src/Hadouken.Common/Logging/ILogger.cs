using System;

namespace Hadouken.Common.Logging
{
    public interface ILogger<T>
    {
        void Log(LogLevel logLevel, string message, params object[] propertyValues);

        void Log(LogLevel logLevel, Exception exception, string message, params object[] propertyValues);
    }
}
