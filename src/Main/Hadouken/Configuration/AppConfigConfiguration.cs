using System;
using System.Configuration;
using System.Globalization;

namespace Hadouken.Configuration
{
    public class AppConfigConfiguration : IConfiguration
    {
        private readonly System.Configuration.Configuration _configuration;

        public AppConfigConfiguration(string configurationFile)
        {
            var configFile = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configurationFile
            };

            _configuration = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
        }

        public string ApplicationDataPath
        {
            get { return Get("ApplicationDataPath"); }
            set { Set("ApplicationDataPath", value); }
        }

        public string InstanceName
        {
            get { return Get("InstanceName"); }
            set { Set("InstanceName", value); }
        }

        public string HostBinding
        {
            get { return Get("HostBinding"); }
            set { Set("HostBinding", value); }
        }

        public int Port
        {
            get { return Convert.ToInt32(Get("Port")); }
            set { Set("Port", value.ToString(CultureInfo.InvariantCulture)); }
        }

        public string UserName
        {
            get { return Get("UserName"); }
            set { Set("UserName", value); }
        }

        public string Password
        {
            get { return Get("Password"); }
            set { Set("Password", value); }
        }

        public string PluginDirectory
        {
            get { return Get("PluginDirectory"); }
            set { Set("PluginDirectory", value); }
        }

        public string PluginUrlTemplate
        {
            get { return Get("PluginUrlTemplate"); }
            set { Set("PluginUrlTemplate", value); }
        }

        public string PluginRepositoryUrl
        {
            get { return Get("PluginRepositoryUrl"); }
            set { Set("PluginRepositoryUrl", value); }
        }

        public void Save()
        {
            _configuration.Save();
        }

        private string Get(string key)
        {
            return _configuration.AppSettings.Settings[key].Value;
        }

        private void Set(string key, string value)
        {
            _configuration.AppSettings.Settings[key].Value = value;
        }
    }
}