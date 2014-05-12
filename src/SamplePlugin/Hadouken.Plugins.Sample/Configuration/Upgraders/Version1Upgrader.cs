using Hadouken.Fx.Configuration;

namespace Hadouken.Plugins.Sample.Configuration.Upgraders
{
    [Version(1)]
    public class Version1Upgrader : VersionUpgrader<SampleConfig>
    {
        public override SampleConfig Upgrade(dynamic data)
        {
            return new SampleConfig
            {
                Bar = (int)data.Bar,
                Foo = "Upgraded: " + data.Foo
            };
        }
    }
}
