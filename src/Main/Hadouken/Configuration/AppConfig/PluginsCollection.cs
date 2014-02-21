using System;
using System.Configuration;
using System.IO;

namespace Hadouken.Configuration.AppConfig
{
    public class PluginsCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("baseDirectory")]
        public string BaseDirectory
        {
            get { return GetFullPath(this["baseDirectory"].ToString()); }
            set { this["baseDirectory"] = value; }
        }

        [ConfigurationProperty("repositoryUri")]
        public Uri RepositoryUri
        {
            get { return new Uri(this["repositoryUri"].ToString()); }
            set { this["repositoryUri"] = value.ToString(); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var pluginElement = (PluginElement)element;
            return pluginElement.Id;
        }

        private string GetFullPath(string relativePath)
        {
            string path = Environment.ExpandEnvironmentVariables(relativePath);
            path = Path.GetFullPath(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
