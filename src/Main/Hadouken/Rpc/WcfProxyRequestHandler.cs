using System;
using System.Collections.Generic;
using System.ServiceModel;

using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;

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
                    var result = proxy.Call(request.Serialize());

                    Exception responseParseException;
                    JsonRpcResponse response;

                    if (JsonRpcResponse.TryParse(result, out response, out responseParseException))
                    {
                        _proxyList.Add(plugin, proxy);
                        return response;
                    }

                    _proxyList.Add(plugin, null);
                }
                catch (Exception exception)
                {
                    return new JsonRpcResponse
                    {
                        Id = request.Id,
                        Error = new InternalRpcError() {Data = exception.Message}
                    };
                }
            }
            else if(_proxyList[plugin] != null)
            {
                var wcfProxy = _proxyList[plugin];
                var result = wcfProxy.Call(request.Serialize());

                Exception parseException;
                JsonRpcResponse response;

                if (JsonRpcResponse.TryParse(result, out response, out parseException)) return response;

                // This proxy does not send valid responses.
                _proxyList[plugin] = null;
                //TODO: Log

                return new JsonRpcResponse{Id = request.Id, Error = new InternalRpcError()};
            }

            return new JsonRpcResponse {Id = request.Id, Error = new MethodNotFoundError()};
        }
    }
}
