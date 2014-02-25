using System.Collections.Generic;

namespace Hadouken.Plugins
{
    public interface IPluginEngine
    {
        IEnumerable<IPluginManager> GetAll();

        IPluginManager Get(string name);

        /// <summary>
        /// Scans the base directory for new plugins. Will not load anything.
        /// </summary>
        void Scan();

        /// <summary>
        /// Load all plugins which are in the 'Unloaded' state.
        /// </summary>
        void LoadAll();

        /// <summary>
        /// Unload all plugins which are in the 'Loaded' state.
        /// </summary>
        void UnloadAll();

        /// <summary>
        /// Loads the specific plugin. This will download missing dependencies if there are any.
        /// </summary>
        /// <param name="name">The plugin to load.</param>
        void Load(string name);

        bool CanLoad(string name, out string[] missingDependencies);

        /// <summary>
        /// Unloads the specific plugin.
        /// </summary>
        /// <param name="name">The plugin to unload.</param>
        void Unload(string name);

        void InstallOrUpgrade(IPackage package);

        /// <summary>
        /// Uninstalls the plugin. The plugin must be in the Unloaded state.
        /// </summary>
        /// <param name="name">The plugin to uninstall.</param>
        void Uninstall(string name);
    }
}
