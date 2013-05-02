using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Configuration;
using Microsoft.Win32;
using Hadouken.Common;

namespace Hadouken.Impl.Config
{
    [Component(ComponentType.Singleton)]
    public class DefaultRegistryReader : IRegistryReader
    {
        private readonly RegistryKey _registryKey = Registry.LocalMachine.OpenSubKey("Software\\Hadouken");

        public string ReadString(string key, string defaultValue = null)
        {
            if (_registryKey != null && _registryKey.GetValue(key) != null)
                return _registryKey.GetValue(key) as string;

            return defaultValue;
        }

        public int ReadInt(string key, int defaultValue = -1)
        {
            if (_registryKey != null && _registryKey.GetValue(key) != null)
                return Convert.ToInt32(_registryKey.GetValue(key));

            return defaultValue;
        }
    }
}
