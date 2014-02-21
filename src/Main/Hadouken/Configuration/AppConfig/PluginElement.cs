using System;
using System.Configuration;
using System.IO;

namespace Hadouken.Configuration.AppConfig
{
    public class PluginElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        public string Id
        {
            get { return this["id"].ToString(); }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return GetFullPath(this["path"].ToString()); }
            set { this["path"] = value; }
        }

        private string GetFullPath(string relativePath)
        {
            relativePath = relativePath.Replace("${Configuration}", GetConfiguration());

            string path = Environment.ExpandEnvironmentVariables(relativePath);
            path = System.IO.Path.GetFullPath(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        private string GetConfiguration()
        {
#if DEBUG
            return "Debug";
#else
            return "Release";
#endif
        }
    }
}
