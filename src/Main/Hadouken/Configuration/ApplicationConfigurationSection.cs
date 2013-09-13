using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Configuration
{
    public class ApplicationConfigurationSection : ConfigurationSection, IConfiguration
    {
        [ConfigurationProperty("instanceName", IsRequired = false, DefaultValue = "Hadouken")]
        public string InstanceName
        {
            get { return this["instanceName"].ToString(); }
            set { this["instanceName"] = value; }
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

        public void Save()
        {
            throw new NotImplementedException();
        }

        public static IConfiguration Load()
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = cfg.GetSection("hdkn") as ApplicationConfigurationSection;

            return section;
        }
    }
}
