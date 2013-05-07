using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Messaging;

namespace Hadouken.Messages
{
    public interface IPluginLoading : IMessage
    {
        string PluginName { get; set; }
        Version PluginVersion { get; set; }
        Type PluginType { get; set; }
    }
}
