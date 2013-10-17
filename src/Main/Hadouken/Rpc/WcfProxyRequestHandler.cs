using System;
using System.Collections.Generic;
using System.ServiceModel;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using NLog;

namespace Hadouken.Rpc
{
    public class WcfProxyRequestHandler : RequestHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDictionary<string, IPluginManagerService> _proxyList = new Dictionary<string, IPluginManagerService>();

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

                case "torrents":
                    plugin = "core.torrents";
                    break;
            }

            if (!_proxyList.ContainsKey(plugin))
            {
                var binding = new NetNamedPipeBinding
                {
                    MaxBufferPoolSize = 10485760,
                    MaxBufferSize = 10485760,
                    MaxConnections = 10,
                    MaxReceivedMessageSize = 10485760
                };

                // Create proxy
                var factory = new ChannelFactory<IPluginManagerService>(binding,
                    "net.pipe://localhost/hdkn.plugins." + plugin);
                var proxy = factory.CreateChannel();

                try
                {
                    var result = proxy.RpcAsync(request.Serialize()).Result;

                    Exception responseParseException;
                    JsonRpcResponse response;

                    if (JsonRpcResponse.TryParse(result, out response, out responseParseException))
                    {
                        if (!_proxyList.ContainsKey(plugin))
                            _proxyList.Add(plugin, proxy);

                        return response;
                    }

                    _proxyList.Add(plugin, null);
                }
                catch (Exception exception)
                {
                    return JsonRpcErrorResponse.InternalRpcError(request.Id, exception);
                }
            }
            else if(_proxyList[plugin] != null)
            {
                var wcfProxy = _proxyList[plugin];
                var result = wcfProxy.RpcAsync(request.Serialize()).Result;

                Exception parseException;
                JsonRpcResponse response;

                if (JsonRpcResponse.TryParse(result, out response, out parseException)) return response;

                // This proxy does not send valid responses. Remove it from our known proxies.
                _proxyList.Remove(plugin);

                Logger.Error("Received invalid response from proxy {0}", plugin);
                return JsonRpcErrorResponse.InternalRpcError(request.Id);
            }

            return JsonRpcErrorResponse.MethodNotFound(request.Id, request.Method);
        }
    }
}
