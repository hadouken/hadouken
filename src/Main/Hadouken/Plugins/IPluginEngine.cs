using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hadouken.Plugins
{
    public interface IPluginEngine
    {
        IEnumerable<IPluginManager> GetAll();

        IPluginManager Get(string name);

        void Rebuild();

        /// <summary>
        /// Load all plugins which are in the 'Unloaded' state.
        /// </summary>
        void LoadAll();

        /// <summary>
        /// Unload all plugins which are in the 'Loaded' state.
        /// </summary>
        void UnloadAll();

        void Load(string name);

        void Unload(string name);

        void InstallOrUpgrade(IPackage package);

        /// <summary>
        /// Removes the plugin from the known plugins. The plugin must be in the Unloaded state.
        /// </summary>
        /// <param name="name">The plugin to remove.</param>
        void Remove(string name);
    }
}
