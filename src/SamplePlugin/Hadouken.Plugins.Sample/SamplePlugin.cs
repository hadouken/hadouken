using Hadouken.Fx;
using Hadouken.Fx.Bootstrapping.TinyIoC;

namespace Hadouken.Plugins.Sample
{
    [PluginBootstrapper(typeof(TinyIoCBootstrapper))]
    public class SamplePlugin : Plugin
    {
        public override void Load()
        {
        }
    }
}
