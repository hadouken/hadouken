using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;

namespace Hadouken.Plugins.Events
{
    public class EventsPlugin : Plugin
    {
        private readonly IEventServer _eventServer;
        private readonly IJsonRpcServer _rpcServer;

        public EventsPlugin(IEventServer eventServer, IJsonRpcServer rpcServer)
        {
            _eventServer = eventServer;
            _rpcServer = rpcServer;
        }

        public override void Load()
        {
            _eventServer.Start();
            _rpcServer.Open();
        }

        public override void Unload()
        {
            _rpcServer.Close();
            _eventServer.Stop();
        }
    }
}
