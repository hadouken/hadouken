using Hadouken.Fx.IO;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Metadata;
using NuGet;

namespace Hadouken.Plugins
{
    public interface IPluginManager
    {
        IDirectory BaseDirectory { get; }

        IPackage Package { get; }

        IManifest Manifest { get; }

        PluginState State { get; }

        IIsolatedEnvironment IsolatedEnvironment { get; }

        int ErrorCount { get; }

        string ErrorMessage { get; }

        long GetMemoryUsage();

        void Load();

        void Unload();
    }
}
