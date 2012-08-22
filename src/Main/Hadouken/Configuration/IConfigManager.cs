using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Configuration
{
    public interface IConfigManager
    {
        string ConnectionString { get; }
        string[] AllKeys { get; }
        string this[string key] { get; }
    }
}
