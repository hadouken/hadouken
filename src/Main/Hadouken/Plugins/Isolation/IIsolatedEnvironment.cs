using System;
using Hadouken.Fx;

namespace Hadouken.Plugins.Isolation
{
    public interface IIsolatedEnvironment
    {
        event EventHandler UnhandledError;

        void Load(PluginConfiguration configuration);

        void Unload();

        long GetMemoryUsage();
    }
}
