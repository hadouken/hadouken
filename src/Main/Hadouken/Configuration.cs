using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.IO;

namespace Hadouken
{
    public class Configuration : IConfiguration
    {
        private readonly IFileSystem _fileSystem;

        public Configuration(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string PluginsPath
        {
            get { return GetPath("Path.Plugins"); }
        }

        public string HostBinding
        {
            get { return ConfigurationManager.AppSettings["HostBinding"]; }
        }

        public int Port
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["Port"]); }
        }

        private string GetPath(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            _fileSystem.CreateDirectory(value);

            return value;
        }
    }
}
