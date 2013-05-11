using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Messaging;
using System.Reflection;

namespace Hadouken.Messages
{
    public interface IPluginLoading : IMessage
    {
        string Name { get; set; }
        Version Version { get; set; }
        Assembly[] Assemblies { get; set; }
    }
}
