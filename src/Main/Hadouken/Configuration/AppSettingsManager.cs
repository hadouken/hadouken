using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Hadouken.Configuration
{
    public class AppSettingsManager : IConfigManager
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["hdkn"].ConnectionString; }
        }

        public string[] AllKeys
        {
            get { return ConfigurationManager.AppSettings.AllKeys; }
        }

        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        }
    }
}
