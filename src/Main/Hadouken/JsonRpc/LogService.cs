using System.Collections.Generic;
using System.Linq;
using Hadouken.Fx.JsonRpc;
using Hadouken.Logging;
using Serilog;
using Serilog.Events;

namespace Hadouken.JsonRpc
{
    public class LogService : IJsonRpcService
    {
        private readonly ILogger _logger;
        private readonly IInMemorySink _memorySink;

        public LogService(ILogger logger, IInMemorySink memorySink)
        {
            _logger = logger;
            _memorySink = memorySink;
        }

        [JsonRpcMethod("core.logging.log")]
        public bool Log(string source, int logLevel, string message)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            var level = TranslateLogLevel(logLevel);
            _logger.ForContext("PluginId", source).Write(level, message);

            return true;
        }

        [JsonRpcMethod("core.logging.getEntries")]
        public object[] GetEntries()
        {
            return (from ev in _memorySink.LogEvents
                orderby ev.Timestamp ascending 
                select new
                {
                    ev.Timestamp,
                    Level = ev.Level.ToString(),
                    Message = ev.RenderMessage(),
                    ev.Exception
                } as object).ToArray();
        }

        private LogEventLevel TranslateLogLevel(int level)
        {
            return (LogEventLevel) level;
        }
    }
}
