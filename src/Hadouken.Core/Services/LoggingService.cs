using System;
using System.Collections.Generic;
using Hadouken.Common.JsonRpc;
using Hadouken.Common.Logging;

namespace Hadouken.Core.Services
{
    public class LoggingService : IJsonRpcService
    {
        private readonly ILoggerRepository _loggerRepository;

        public LoggingService(ILoggerRepository loggerRepository)
        {
            if (loggerRepository == null) throw new ArgumentNullException("loggerRepository");
            _loggerRepository = loggerRepository;
        }

        [JsonRpcMethod("logging.getEntries")]
        public IEnumerable<LogEntry> GetEntries()
        {
            return _loggerRepository.GetAll();
        }
    }
}
