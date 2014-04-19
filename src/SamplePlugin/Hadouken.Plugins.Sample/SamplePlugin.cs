using Hadouken.Fx;
using Hadouken.Fx.Events;
using Hadouken.Fx.Logging;

namespace Hadouken.Plugins.Sample
{
    public class SamplePlugin : Plugin
    {
        private readonly ILogger _logger;
        private readonly IEventListener _eventListener;

        public SamplePlugin(PluginConfiguration configuration, ILogger logger, IEventListener eventListener)
        {
            _logger = logger;
            _eventListener = eventListener;
        }

        public override void Load()
        {
            _logger.Info("SamplePlugin loading.");

            _eventListener.RegisterHandler("test.handler", (dynamic _) => OnWhaevs());
        }

        private void OnWhaevs()
        {
            _logger.Info("Whaevs happened!");
        }
    }
}
