using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public interface IPluginFactory : IComponent
    {
        void ScanForChanges();

        void Load(string name);
        void LoadAll();

        void Unload(string name);
        void UnloadAll();
    }
}
