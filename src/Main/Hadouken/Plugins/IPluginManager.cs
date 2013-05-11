using System;

namespace Hadouken.Plugins
{
    public interface IPluginManager
    {
        string Name { get; }
        Version Version { get; }

        void Load();
        void Unload();

        byte[] GetResource(string name);
    }
}
