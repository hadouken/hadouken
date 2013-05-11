using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Hadouken.Plugins
{
    public interface IPluginLoader
    {
        bool CanLoad(string path);
        IEnumerable<Type> Load(string path);
    }
}
