using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Http.Models
{
    public class PutPluginDto
    {
        public PluginAction Action { get; set; }
    }

    public enum PluginAction
    {
        Load,
        Unload
    }
}
