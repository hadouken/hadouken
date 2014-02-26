using Hadouken.Fx;
using Hadouken.Fx.Bootstrapping;
using Hadouken.Plugins.Sample.Bootstrapping;

namespace Hadouken.Plugins.Sample
{
    [Bootstrapper(typeof(CustomBootstrapper))]
    public class SamplePlugin : Plugin
    {
        public override void Load()
        {
        }
    }
}
