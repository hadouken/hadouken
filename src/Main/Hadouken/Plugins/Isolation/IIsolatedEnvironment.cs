using Hadouken.Fx;

namespace Hadouken.Plugins.Isolation
{
    public interface IIsolatedEnvironment
    {
        void Load(PluginConfiguration configuration);

        void Unload();

        long GetMemoryUsage();
    }
}
