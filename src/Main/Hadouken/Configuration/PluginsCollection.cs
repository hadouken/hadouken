using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Configuration
{
    public class PluginsCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("baseDirectory")]
        public string BaseDirectory
        {
            get { return GetFullPath(this["baseDirectory"].ToString()); }
            set { this["baseDirectory"] = value; }
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
