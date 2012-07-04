using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Logging
{
    public interface ILogger : IComponent
    {
        void Log(string message, params object[] parameters);
        void Trace(string message, params object[] parameters);
        void Debug(string message, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Warn(string message, params object[] parameters);
        void Error(string message, params object[] parameters);
        void Fatal(string message, params object[] parameters);
    }
}
