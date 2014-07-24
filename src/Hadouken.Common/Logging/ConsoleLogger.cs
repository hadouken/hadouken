using System;

namespace Hadouken.Common.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly IConsole _console;

        public ConsoleLogger(IConsole console)
        {
            if (console == null) throw new ArgumentNullException("console");
            _console = console;
        }

        public void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            var msg = string.Format("{0}: {1}", logLevel.ToString().ToUpper(), message);

            if (exception != null)
            {
                msg = string.Concat(msg, Environment.NewLine, "\t", exception.ToString());
            }

            _console.WriteLine(msg);
        }
    }
}