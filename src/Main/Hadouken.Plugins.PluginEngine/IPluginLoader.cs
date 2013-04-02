using System;
using System.Collections.Generic;

namespace Hadouken.Plugins.PluginEngine
{
    public interface IPluginLoader : IComponent
    {
        bool CanLoad(string path);
        IEnumerable<byte[]> Load(string path);
    }
}
