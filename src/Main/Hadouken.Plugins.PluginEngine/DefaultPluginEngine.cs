using System;

using NLog;
using Hadouken.Common;

namespace Hadouken.Plugins.PluginEngine
{
    [Component]
    public class DefaultPluginEngine : IPluginEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Load()
        {
            //
        }

        public void Load(string path)
        {
            //
        }

        public void UnloadAll()
        {
            //
        }
    }
}
