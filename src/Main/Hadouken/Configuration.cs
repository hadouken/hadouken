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

        private string GetPath(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            _fileSystem.CreateDirectory(value);

            return value;
        }
    }
}
