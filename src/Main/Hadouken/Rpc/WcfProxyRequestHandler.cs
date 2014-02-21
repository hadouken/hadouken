using System;
using System.Collections.Generic;
using System.ServiceModel;
using Hadouken.Configuration;
using Hadouken.Framework.Events;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Wcf;
using NLog;

namespace Hadouken.Rpc
{
    public class WcfProxyRequestHandler : RequestHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IEventListener _eventListener;
        private readonly IProxyFactory<IPluginManagerService> _proxyFactory;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDictionary<string, IPluginManagerService> _proxyList = new Dictionary<string, IPluginManagerService>();

        public WcfProxyRequestHandler(IEnumerable<IJsonRpcService> services,
            IConfiguration configuration,
            IEventListener eventListener,
            IProxyFactory<IPluginManagerService> proxyFactory)
            : base(services)
        {
            _configuration = configuration;
            _eventListener = eventListener;
            _proxyFactory = proxyFactory;
            _eventListener.Subscribe<string>("plugin.unloaded", OnPluginUnloaded);
        }

        private void OnPluginUnloaded(string name)
        {
            if (_proxyList.ContainsKey(name))
            {
                _proxyList.Remove(name);
            }
        }

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

            var endpoint = new Uri(string.Format(_configuration.Rpc.PluginUriTemplate, plugin));

            try
            {
                using (var proxy = _proxyFactory.Create(endpoint))
                {
                    var serializedRequest = request.Serialize();
                    var result = proxy.Channel.RpcAsync(serializedRequest).Result;

                    JsonRpcResponse response;
                    Exception exception;

                    if (JsonRpcResponse.TryParse(result, out response, out exception))
                    {
                        return response;
                    }

                    Logger.ErrorException("Could not parse JSONRPC response.", exception);

                    return JsonRpcErrorResponse.ParseError(request.Id);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error when calling JSONRPC service.", e);
                return JsonRpcErrorResponse.InternalRpcError(request.Id, e);
            }
        }
    }
}
