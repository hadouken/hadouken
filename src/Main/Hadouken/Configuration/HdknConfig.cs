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
        private static bool _isInitialized = false;

        static HdknConfig()
        {
            ConfigManager = new AppSettingsManager();
        }

        public static IConfigManager ConfigManager { get; set; }

        private static void Initialize()
        {
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (key.StartsWith("Paths"))
                {   
                    CreatePath(ExpandPath(ConfigManager[key]));
                }
            }
        }

        private static string ExpandPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return null;

            var asm = Assembly.GetEntryAssembly();
            if (asm != null)
                path = path.Replace("$BaseDir$", Path.GetDirectoryName(asm.Location));

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
            if (!_isInitialized)
                Initialize();

            string path = ConfigManager[key];
            return ExpandPath(path);
        }

        public static string ConnectionString
        {
            get
            {
                if (!_isInitialized)
                    Initialize();

                return ConfigManager.ConnectionString;
            }
        }
    }
}
