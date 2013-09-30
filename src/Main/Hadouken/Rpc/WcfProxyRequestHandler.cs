using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;
using Newtonsoft.Json;

namespace Hadouken.Rpc
{
    public class WcfProxyRequestHandler : RequestHandler
    {
        private readonly IDictionary<string, IWcfJsonRpcServer> _proxyList = new Dictionary<string, IWcfJsonRpcServer>();

        public WcfProxyRequestHandler(IEnumerable<IJsonRpcService> services) : base(services) { }

        protected override JsonRpcResponse OnMethodMissing(JsonRpcRequest request)
        {
            var parts = request.Method.Split('.');
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
                    var result = proxy.Call(JsonConvert.SerializeObject(request));

                    if (!String.IsNullOrEmpty(result))
                    {
                        _proxyList.Add(plugin, proxy);
                        return JsonConvert.DeserializeObject<JsonRpcResponse>(result);
                    }
                    
                    _proxyList.Add(plugin, null);
                }
                catch(Exception) {}
            }
            else if(_proxyList[plugin] != null)
            {
                var wcfProxy = _proxyList[plugin];
                var result = wcfProxy.Call(JsonConvert.SerializeObject(request));
                return JsonConvert.DeserializeObject<JsonRpcResponse>(result);
            }

            return null;
        }
    }
}
