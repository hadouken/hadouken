using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;

namespace Hadouken.Configuration
{
    public static class HdknConfig
    {
        static HdknConfig()
        {
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (key.StartsWith("Paths"))
                {
                    CreatePath(ExpandPath(ConfigurationManager.AppSettings[key]));
                }
            }
        }

        private static string ExpandPath(string path)
        {
            return Environment.ExpandEnvironmentVariables(Path.GetFullPath(path));
        }

        private static void CreatePath(string path)
        {
            // create it if it doesn't exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string GetPath(string key)
        {
            string path = ConfigurationManager.AppSettings[key];
            return ExpandPath(path);
        }

        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["hdkn"].ConnectionString; }
        }
    }
}
