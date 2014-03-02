using System.Collections.Generic;

namespace Hadouken.Plugins
{
    public interface IPluginEngine
    {
        /// <summary>
        /// Gets all <see cref="IPluginManager"/> that are associated with this plugin engine.
        /// </summary>
        IEnumerable<IPluginManager> GetAll();

        /// <summary>
        /// Get a specific <see cref="IPluginManager"/> or null if it was not found.
        /// </summary>
        /// <param name="name">The name of the <see cref="IPluginManager"/> to get.</param>
        IPluginManager Get(string name);

        /// <summary>
        /// Scans the base directory for new plugins. Will not load anything.
        /// </summary>
        void Scan();

        /// <summary>
        /// Loads all available <see cref="IPluginManager"/> which are in the 'Unloaded' state.
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

        /// <summary>
        /// Check if a specific <see cref="IPluginManager"/> have all dependencies available for it to load.
        /// </summary>
        /// <param name="name">The name of the <see cref="IPluginManager"/></param>
        /// <param name="missingDependencies">A list of the known dependencies which are missing</param>
        /// <returns>True if the <see cref="IPluginManager"/> can load. Otherwise false.</returns>
        bool CanLoad(string name, out string[] missingDependencies);

        bool CanUnload(string name, out string[] dependencies);

        /// <summary>
        /// Unloads the specific plugin.
        /// </summary>
        /// <param name="name">The plugin to unload.</param>
        void Unload(string name);

        /// <summary>
        /// Installs or upgrades the provided package. If the package upgrades an
        /// existing plugin, the plugin will be unloaded and then uninstalled before
        /// the new version.
        /// </summary>
        /// <param name="package">The <see cref="IPackage"/> to install or upgrade.</param>
        /// <returns>True if the install/upgrade was successful. Otherwise false.</returns>
        bool InstallOrUpgrade(IPackage package);

        /// <summary>
        /// Uninstalls the plugin. The plugin must be in the Unloaded state.
        /// </summary>
        /// <param name="name">The plugin to uninstall.</param>
        bool Uninstall(string name);
    }
}
