using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Data;

namespace Hadouken.Data.Models
{
    public class PluginInfo : Model
    {
        public virtual string Path { get; set; }
        public virtual string Name { get; set; }
        public virtual string Version { get; set; }
    }
}
