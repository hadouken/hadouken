using Hadouken.Fx;
using Hadouken.Fx.Logging;

namespace Hadouken.Plugins.Sample
{
    public class SamplePlugin : Plugin
    {
        private readonly ILogger _logger;

        public SamplePlugin(PluginConfiguration configuration, ILogger logger)
        {
            _logger = logger;
        }

        public override void Load()
        {
            _logger.Info("SamplePlugin loading.");
        }
    }
}
