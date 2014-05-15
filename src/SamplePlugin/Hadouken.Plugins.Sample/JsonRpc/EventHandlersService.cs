using Hadouken.Fx.JsonRpc;
using Hadouken.Fx.Logging;

namespace Hadouken.Plugins.Sample.JsonRpc
{
    public sealed class EventHandlersService : IJsonRpcService
    {
        private readonly ILogger _logger;

        public EventHandlersService(ILogger logger)
        {
            _logger = logger;
        }

        [JsonRpcMethod("sample.on.customEvent")]
        public void OnCustomEvent(string foo)
        {
            _logger.Info("Custom event received. Taking a long time...");
            System.Threading.Thread.Sleep(5000);
            _logger.Info("All done.");
        }

        [JsonRpcMethod("sample.on.allEvents")]
        public void OnAllEvents(string eventName, object data)
        {
            _logger.Info("Handler for all events invoked. Event name: " + eventName);
        }
    }
}
