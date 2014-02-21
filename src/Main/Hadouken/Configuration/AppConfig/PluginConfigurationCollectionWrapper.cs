using System;
using System.Collections;
using System.Collections.Generic;

namespace Hadouken.Configuration.AppConfig
{
    public class PluginConfigurationCollectionWrapper : IPluginConfigurationCollection
    {
        private readonly PluginsCollection _pluginsCollection;
        private readonly IList<IPluginConfiguration> _pluginConfigurations; 

        public PluginConfigurationCollectionWrapper(PluginsCollection pluginsCollection)
        {
            _pluginsCollection = pluginsCollection;
            _pluginConfigurations = new List<IPluginConfiguration>();

            foreach (PluginElement plugin in pluginsCollection)
            {
                _pluginConfigurations.Add(new PluginElementWrapper(plugin));
            }
        }

        public IEnumerator<IPluginConfiguration> GetEnumerator()
        {
            return _pluginConfigurations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pluginConfigurations.GetEnumerator();
        }

        public string BaseDirectory
        {
            get { return _pluginsCollection.BaseDirectory; }
            set { _pluginsCollection.BaseDirectory = value; }
        }

        public Uri RepositoryUri
        {
            get { return _pluginsCollection.RepositoryUri; }
            set { _pluginsCollection.RepositoryUri = value; }
        }
    }
}
