using System;

namespace Hadouken.Common.Logging
{
    public static class LoggerExtensions
    {
        public static void Trace(this ILogger logger, string message, Exception e = null)
        {
            logger.Log(LogLevel.Trace, message, e);
        }

        public static void Debug(this ILogger logger, string message, Exception e = null)
        {
            logger.Log(LogLevel.Debug, message, e);
        }

        public static void Info(this ILogger logger, string message, Exception e = null)
        {
            logger.Log(LogLevel.Info, message, e);
        }

        public static void Warn(this ILogger logger, string message, Exception e = null)
        {
            logger.Log(LogLevel.Warn, message, e);
        }

        public static void Error(this ILogger logger, string message, Exception e = null)
        {
            logger.Log(LogLevel.Error, message, e);
        }

        public static void Fatal(this ILogger logger, string message, Exception e = null)
        {
            logger.Log(LogLevel.Fatal, message, e);
        }
    }
}
