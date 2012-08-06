using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using System.Reflection;

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
            path = path.Replace("$BaseDir$", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            if (String.IsNullOrEmpty(path))
                return null;

            return Environment.ExpandEnvironmentVariables(path);
        }

        private static void CreatePath(string path)
        {
            try
            {
                // create it if it doesn't exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("CreatePath failed with path {0}", path), e);
            }
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
