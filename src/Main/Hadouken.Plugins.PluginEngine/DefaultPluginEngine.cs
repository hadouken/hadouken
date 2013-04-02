using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Plugins.PluginEngine
{
    public class DefaultPluginEngine : IPluginEngine
    {
        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void UnloadAll()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IPluginManager> Managers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
