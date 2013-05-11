using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Configuration
{
    public interface IRegistryReader
    {
        string ReadString(string key, string defaultValue = null);
        int ReadInt(string key, int defaultValue = -1);
    }
}
