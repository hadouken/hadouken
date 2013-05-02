using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Plugins
{
    public interface IPluginEnvironment
    {
        string ConnectionString { get; }
        string DataFolder { get; }
    }
}
