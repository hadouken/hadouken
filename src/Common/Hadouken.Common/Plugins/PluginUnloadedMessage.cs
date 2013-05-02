using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.Plugins
{
    public class PluginUnloadedMessage : Message
    {
        public string Name { get; set; }

        public Version Version { get; set; }
    }
}
