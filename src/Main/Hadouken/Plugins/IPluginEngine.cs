using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hadouken.Plugins
{
    public interface IPluginEngine
    {
        IEnumerable<IPluginManager> GetAll();

        IPluginManager Get(string name);

        /// <summary>
        /// Scan the plugin base directory for new plugins.
        /// </summary>
        Task ScanAsync();

        /// <summary>
        /// Load all plugins which are in the 'Unloaded' state.
        /// </summary>
        Task LoadAllAsync();

        /// <summary>
        /// Unload all plugins which are in the 'Loaded' state.
        /// </summary>
        Task UnloadAllAsync();

        Task LoadAsync(string name);

        Task UnloadAsync(string name);

        void InstallOrUpgrade(IPackage package);

        /// <summary>
        /// Async removes the plugin from the known plugins. The plugin must be in the Unloaded state.
        /// </summary>
        /// <param name="name">The plugin to remove.</param>
        Task RemoveAsync(string name);
    }
}
