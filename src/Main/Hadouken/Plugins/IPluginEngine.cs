using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins
{
    public interface IPluginEngine
    {
        IEnumerable<PluginInfo> Plugins { get; } 

        /// <summary>
        /// Load all plugins in the default plugin folder.
        /// </summary>
        void Load();

        /// <summary>
        /// Load a plugin from the path specified.
        /// </summary>
        /// <param name="path">The path to a plugin.</param>
        void Load(string path);

        /// <summary>
        /// Unloads the plugin with the specified name.
        /// </summary>
        /// <param name="name">The name of the plugin to unload.</param>
        void Unload(string name);

        void UnloadAll();
    }
}
