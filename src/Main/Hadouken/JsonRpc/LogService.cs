using Hadouken.Fx.JsonRpc;
using Serilog;
using Serilog.Events;

namespace Hadouken.JsonRpc
{
    public class LogService : IJsonRpcService
    {
        private readonly ILogger _logger;

        public LogService(ILogger logger)
        {
            _logger = logger;
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

        private LogEventLevel TranslateLogLevel(int level)
        {
            return (LogEventLevel) level;
        }
    }
}
