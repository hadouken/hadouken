using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using NLog;

namespace HdknPlugins.CreateTorrent
{
    [Plugin("createtorrent", "1.0", ResourceBase = "HdknPlugins.CreateTorrent.UI")]
    public class CreateTorrentPlugin : IPlugin
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Load()
        {
            Logger.Trace("Loading CreateTorrentPlugin");
        }

        public void Unload()
        {
            Logger.Trace("Unloading CreateTorrentPlugin");
        }
    }
}
