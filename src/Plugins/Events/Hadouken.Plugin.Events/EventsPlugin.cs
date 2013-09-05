using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins.Events
{
    public class EventsPlugin : Plugin
    {
        private readonly IEventServer _eventServer;
        private readonly IJsonRpcServer _jsonRpcServer;

        public EventsPlugin(IEventServer eventServer, IJsonRpcServer jsonRpcServer)
        {
            _eventServer = eventServer;
            _jsonRpcServer = jsonRpcServer;
        }

        public override void Load()
        {
            _eventServer.Start();
            _jsonRpcServer.Start();
        }

        public override void Unload()
        {
            _jsonRpcServer.Stop();
            _eventServer.Stop();
        }
    }
}
