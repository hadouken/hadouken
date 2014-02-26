using System;
using System.Collections.Generic;
using System.ServiceModel;
using Hadouken.Fx.ServiceModel;

namespace Hadouken.Fx.JsonRpc
{
    public class WcfClientTransport : IClientTransport
    {
        private readonly string _uriTemplate;
        private readonly IJsonSerializer _jsonSerializer;

        public WcfClientTransport(string uriTemplate, IJsonSerializer jsonSerializer)
        {
            _uriTemplate = uriTemplate;
            _jsonSerializer = jsonSerializer;
        }

        public string Call(string json)
        {
            var plugin = ExtractPluginName(json);
            var binding = new NetNamedPipeBinding();
            var endpoint = new EndpointAddress(string.Format(_uriTemplate, plugin));

            using (var factory = new ChannelFactory<IPluginService>(binding, endpoint))
            {
                var channel = factory.CreateChannel();
                return channel.HandleJsonRpc(json);
            }
        }

        private string ExtractPluginName(string json)
        {
            var obj = (IDictionary<string, object>) _jsonSerializer.Deserialize(json);

            if (!obj.ContainsKey("method"))
            {
                throw new InvalidOperationException("Invalid json");
            }

            var method = obj["method"].ToString();
            return method.Substring(0, method.IndexOf('.'));
        }
    }
}