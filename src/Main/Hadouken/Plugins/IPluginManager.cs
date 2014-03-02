using Hadouken.Fx.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPluginManager
    {
        IDirectory BaseDirectory { get; }

        IManifest Manifest { get; }

        PluginState State { get; }

        string ErrorMessage { get; }

        long GetMemoryUsage();

        void Load();

        void Unload();
    }
}
