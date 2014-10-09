using System;

namespace Hadouken.Common.Logging
{
    public static class LoggerExtensions
    {
        public static void Trace<T>(this ILogger<T> logger, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Trace, message, propertyValues);
        }

        public static void Debug<T>(this ILogger<T> logger, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Debug, message, propertyValues);
        }

        public static void Info<T>(this ILogger<T> logger, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Info, message, propertyValues);
        }

        public static void Info<T>(this ILogger<T> logger, Exception exception, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Info, exception, message, propertyValues);
        }

        public static void Warn<T>(this ILogger<T> logger, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Warn, message, propertyValues);
        }

        public static void Warn<T>(this ILogger<T> logger, Exception exception, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Warn, exception, message, propertyValues);
        }

        public static void Error<T>(this ILogger<T> logger, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Error, message, propertyValues);
        }

        public static void Error<T>(this ILogger<T> logger, Exception exception, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Error, exception, message, propertyValues);
        }

        public static void Fatal<T>(this ILogger<T> logger, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Fatal, message, propertyValues);
        }

        public static void Fatal<T>(this ILogger<T> logger, Exception exception, string message, params object[] propertyValues)
        {
            logger.Log(LogLevel.Fatal, exception, message, propertyValues);
        }
    }
}
