using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Metadata
{
    public sealed class Manifest
    {
        public string Name { get; set; }

        public Version Version { get; set; }

        public Dependency[] Dependencies { get; set; }
    }
}
