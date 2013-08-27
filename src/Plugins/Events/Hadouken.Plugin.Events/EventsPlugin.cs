using System;

using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.Events
{
    public class EventsPlugin : Plugin
    {
        private readonly IEventServer _eventServer;

        public EventsPlugin(IEventServer eventServer)
        {
            _eventServer = eventServer;
        }

        public override void Load()
        {
            _eventServer.Start();
        }

        public override void Unload()
        {
            _eventServer.Stop();
        }
    }
}
