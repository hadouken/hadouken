using System;
using System.Collections.Generic;
using System.Linq;

using Hadouken.Configuration;
using Hadouken.IO;
using Hadouken.Plugins.Metadata;
using NLog;

namespace Hadouken.Plugins
{
    public class PluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<IPluginLoader> _pluginLoaders;

        private readonly IDictionary<string, IPluginManager> _pluginManagers =
            new Dictionary<string, IPluginManager>(StringComparer.InvariantCultureIgnoreCase);

        private readonly object _lock = new object();

        public PluginEngine(IConfiguration configuration, IFileSystem fileSystem, IEnumerable<IPluginLoader> pluginLoaders)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
            _pluginLoaders = pluginLoaders;
        }

        public IEnumerable<IPluginManager> GetAll()
        {
            lock (_lock)
            {
                return new List<IPluginManager>(_pluginManagers.Values);
            }
        }

        public IPluginManager Get(string name)
        {
            lock (_lock)
            {
                if (_pluginManagers.ContainsKey(name))
                    return _pluginManagers[name];

                return null;
            }
        }

        public void Load()
        {
            var entries = new List<string>((from PluginElement element
                                                in _configuration.Plugins
                                            select element.Path));

            // Add entries from baseDirectory
            entries.AddRange(
                (from entry in _fileSystem.GetDirectoryEntries(_configuration.Plugins.BaseDirectory)
                 select entry));

            var extraPlugin = GetOptionalPluginArgument(Environment.GetCommandLineArgs());

            if (!String.IsNullOrEmpty(extraPlugin))
                entries.Add(extraPlugin);

            var managers = (from entry in entries
                from loader in _pluginLoaders
                where loader.CanLoad(entry)
                select loader.Load(entry)).ToList();

            var availableManagers = new List<IPluginManager>();

            // For each plugin, check that we can fulfill its dependencies.
            foreach (var manager in managers)
            {
                // Make sure we have all dependencies for this manager
                if ((manager.Manifest.Dependencies == null
                    || !manager.Manifest.Dependencies.Any())

                    || (manager.Manifest.Dependencies != null
                    && manager.Manifest.Dependencies.Any()
                    && manager.Manifest.Dependencies.All(d => CheckDependency(d, managers))))
                {
                    availableManagers.Add(manager);
                }
                else
                {
                    Logger.Error("Not all dependencies are present for plugin {0}", manager.Manifest.Name);
                }
            }

            // Order the list availableManagers by its dependenciess
            var orderedManagers = OrderByDependencies(availableManagers);

            foreach (var manager in orderedManagers)
            {
                LoadPluginManager(manager);
            }
        }

        private static bool CheckDependency(Dependency dependency, IEnumerable<IPluginManager> availableManagers)
        {
            return (from manager in availableManagers
                let manifest = manager.Manifest
                where dependency.Name == manifest.Name
                where dependency.VersionRange.IsIncluded(manifest.Version)
                select manager).Any();
        }

        private static IEnumerable<IPluginManager> OrderByDependencies(ICollection<IPluginManager> managers)
        {
            var nameToInstance = managers.ToDictionary(m => m.Manifest.Name, m => m);
            var nameToDeps = managers.ToDictionary(m => m.Manifest.Name,
                m => m.Manifest.Dependencies.Select(d => d.Name).ToArray());

            var result = new List<IPluginManager>();

            while (nameToDeps.Count > 0)
            {
                // Get all nodes with no dependencies
                var ready = nameToDeps.Where(n => n.Value.Length == 0).Select(n => n.Key).ToList();

                if (!ready.Any())
                    throw new Exception("Circular graph. Plz fix.");

                foreach (var name in ready)
                {
                    nameToDeps.Remove(name);
                }

                foreach (var key in nameToDeps.Keys.ToArray())
                {
                    var deps = nameToDeps[key];
                    deps = deps.Except(ready).ToArray();
                    nameToDeps[key] = deps;
                }

                result.AddRange(ready.Select(n => nameToInstance[n]));
            }

            return result;
        } 

        private static string GetOptionalPluginArgument(string[] args)
        {
            // Args cannot be null and its length must be 2 or more
            // since --plugin and "path" sits in two different positions.

            if (args == null || args.Length < 2)
                return null;

            // Args must contain --plugin

            if (!args.Contains("--plugin"))
                return null;

            var position = Array.IndexOf(args, "--plugin");

            // The array must be at least one field longer
            // than the position of the --plugin entry

            if (position == -1 || args.Length - 1 < position + 1)
                return null;

            return args[position + 1];
        }

        private void LoadPluginManager(IPluginManager manager)
        {
            try
            {
                manager.Load();
            }
            catch (Exception e)
            {
                Logger.ErrorException(String.Format("Could not load plugin {0}", manager.Manifest.Name), e);
                
                manager.Unload();
                return;
            }

            lock (_lock)
            {
                _pluginManagers.Add(manager.Manifest.Name, manager);
            }

            Logger.Info("Plugin {0}[v{1}] loaded", manager.Manifest.Name, manager.Manifest.Version);
        }

        public void Unload()
        {
            lock (_lock)
            {
                foreach (var manager in _pluginManagers.Values)
                {
                    Logger.Info("Unloading plugin {0}", manager.Manifest.Name);

                    try
                    {
                        manager.Unload();
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorException(String.Format("Could not unload plugin {0}", manager.Manifest.Name), e);
                    }
                }

                _pluginManagers.Clear();
            }
        }
    }
}
