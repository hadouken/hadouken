using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Logging;
using System.Diagnostics;

namespace Hadouken.Impl.Logging
{
    public class ConsoleLogger : ILogger
    {
        private string _className;

        public ConsoleLogger()
        {
            _className = "";
        }

        public void Log(string message, params object[] parameters)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();

            Console.WriteLine("LOG[{0}]: {1}", method.DeclaringType.FullName + "." + method.Name, String.Format(message, parameters));
        }

        public void Trace(string message, params object[] parameters)
        {
            Console.WriteLine("TRACE[{0}]: {1}", _className, String.Format(message, parameters));
        }

        public void Debug(string message, params object[] parameters)
        {
            Console.WriteLine("DEBUG[{0}]: {1}", _className, String.Format(message, parameters));
        }

        public void Info(string message, params object[] parameters)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();

            Console.WriteLine("INFO[{0}]: {1}", method.DeclaringType.FullName + "." + method.Name, String.Format(message, parameters));
        }

        public void Warn(string message, params object[] parameters)
        {
            Console.WriteLine("WARN[{0}]: {1}", _className, String.Format(message, parameters));
        }

        public void Error(string message, params object[] parameters)
        {
            Console.WriteLine("ERROR[{0}]: {1}", _className, String.Format(message, parameters));
        }

        public void Fatal(string message, params object[] parameters)
        {
            Console.WriteLine("FATAL[{0}]: {1}", _className, String.Format(message, parameters));
        }
    }
}
