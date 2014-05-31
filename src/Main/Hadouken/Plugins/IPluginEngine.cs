using System.Collections.Generic;
using NuGet;

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

        string[] GetUnloadOrder(string name);

        /// <summary>
        /// Refreshes the plugin graph. Will not load anything.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Loads all available <see cref="IPluginManager"/> which are in the 'Unloaded' state.
        /// </summary>
        void LoadAll();

        /// <summary>
        /// Unload all plugins which are in the 'Loaded' state.
        /// </summary>
        void UnloadAll();

        /// <summary>
        /// Loads the specific plugin.
        /// </summary>
        /// <param name="name">The plugin to load.</param>
        void Load(string name);

        /// <summary>
        /// Unloads the specific plugin.
        /// </summary>
        /// <param name="name">The plugin to unload.</param>
        void Unload(string name);

        void Install(string packageId, string version, bool ignoreDependencies, bool allowPrereleaseVersions);

        void Uninstall(string packageId, string version, bool forceRemove, bool removeDependencies);

        void Update(string packageId, string version, bool updateDependencies, bool allowPrereleaseVersions);
    }
}
