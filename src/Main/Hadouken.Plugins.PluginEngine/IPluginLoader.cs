using System;
using System.Collections.Generic;

namespace Hadouken.Plugins.PluginEngine
{
    public interface IPluginLoader
    {
        bool CanLoad(string path);
        IList<byte[]> Load(string path);
    }
}
