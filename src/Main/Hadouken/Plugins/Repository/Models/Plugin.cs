using System;
using System.Collections.Generic;

namespace Hadouken.Plugins.Repository.Models
{
    public sealed class Plugin
    {
        public Plugin()
        {
            Releases = new List<Release>();
        }

        public string PluginId { get; set; }

        public string Author { get; set; }

        public Uri Homepage { get; set; }

        public string Description { get; set; }

        public IEnumerable<Release> Releases { get; set; }
    }
}
