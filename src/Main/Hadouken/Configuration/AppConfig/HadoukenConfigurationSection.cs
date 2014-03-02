using System;
using System.Configuration;
using System.IO;

namespace Hadouken.Configuration.AppConfig
{
    public class HadoukenConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("instanceName", IsRequired = false, DefaultValue = "Hadouken")]
        public string InstanceName
        {
            get { return this["instanceName"].ToString(); }
            set { this["instanceName"] = value; }
        }

        [ConfigurationProperty("dataPath", IsRequired = false, DefaultValue = "/")]
        public string ApplicationDataPath
        {
            get { return GetFullPath(this["dataPath"].ToString()); }
            set { this["dataPath"] = value; }
        }

        private string GetFullPath(string relativePath)
        {
            string path = Environment.ExpandEnvironmentVariables(relativePath);
            path =  Path.GetFullPath(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        [ConfigurationProperty("plugins", IsRequired = true)]
        [ConfigurationCollection(typeof(PluginsCollection), AddItemName = "plugin", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public PluginsCollection Plugins
        {
            get { return this["plugins"] as PluginsCollection; }
        }

        [ConfigurationProperty("http", IsRequired = true)]
        public HttpConfiguration Http
        {
            get { return this["http"] as HttpConfiguration; }
        }

        [ConfigurationProperty("rpc", IsRequired = true)]
        public RpcConfiguration Rpc
        {
            get { return this["rpc"] as RpcConfiguration; }
        }

        public void Save()
        {
            _configuration.Save();
        }

        private System.Configuration.Configuration _configuration;

        public static IConfiguration Load()
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = cfg.GetSection("hdkn") as HadoukenConfigurationSection;

            if (section == null)
                return null;

            section._configuration = cfg;

            return new ConfigurationWrapper(section);
        }
    }
}
