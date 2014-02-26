using Hadouken.Fx;
using Hadouken.Fx.Bootstrapping;
using Hadouken.Fx.Bootstrapping.TinyIoC;

namespace Hadouken.Plugins.Sample
{
    [Bootstrapper(typeof(TinyIoCBootstrapper))]
    public class SamplePlugin : Plugin
    {
        public override void Load()
        {
        }
    }
}
