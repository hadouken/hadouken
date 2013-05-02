using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Plugins;
using Hadouken.Plugins;
using NLog;
using Hadouken.Common.Messaging;

namespace HdknPlugins.CreateTorrent
{
    public class CreateTorrentPlugin : Plugin
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public CreateTorrentPlugin(IMessageBus messageBus) : base(messageBus)
        {
        }

        public override void Load()
        {
            Logger.Trace("Loading CreateTorrentPlugin");
        }

        public override void Unload()
        {
            Logger.Trace("Unloading CreateTorrentPlugin");
        }
    }
}
