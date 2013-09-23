using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;

namespace Hadouken.Rpc
{
    public class WcfProxyJsonRpcHandler : JsonRpcHandler
    {
        private readonly IDictionary<string, IWcfJsonRpcServer> _proxyList = new Dictionary<string, IWcfJsonRpcServer>();
 
        public WcfProxyJsonRpcHandler(IEnumerable<IJsonRpcService> services) : base(services) {}

        protected override string OnMethodMissing(string methodName, string rawRequest)
        {
            var parts = methodName.Split('.');
            var plugin = parts[0];

            switch (plugin)
            {
                case "config":
                    plugin = "core.config";
                    break;

                case "events":
                    plugin = "core.events";
                    break;

                case "torrents":
                    plugin = "core.torrents";
                    break;
            }

            if (!_proxyList.ContainsKey(plugin))
            {
                // Create proxy
                var factory = new ChannelFactory<IWcfJsonRpcServer>(new NetNamedPipeBinding(),
                    "net.pipe://localhost/hdkn.plugins." + plugin);
                var proxy = factory.CreateChannel();

                try
                {
                    var result = proxy.Call(rawRequest);

                    if (!String.IsNullOrEmpty(result))
                    {
                        _proxyList.Add(plugin, proxy);
                        return result;
                    }
                    
                    _proxyList.Add(plugin, null);
                }
                catch(Exception) {}
            }
            else if(_proxyList[plugin] != null)
            {
                var wcfProxy = _proxyList[plugin];
                return wcfProxy.Call(rawRequest);
            }

            return null;
        }
    }
}
