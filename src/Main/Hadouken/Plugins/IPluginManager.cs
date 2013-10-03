using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPluginManager
    {
        IManifest Manifest { get; }

        PluginState State { get; }

        void Load();

        void Unload();
    }
}
