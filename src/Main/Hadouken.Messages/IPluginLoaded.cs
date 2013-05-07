using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Messaging;
using Hadouken.Plugins;

namespace Hadouken.Messages
{
    public interface IPluginLoaded : IMessage
    {
        IPluginManager Manager { get; set; }
    }
}
