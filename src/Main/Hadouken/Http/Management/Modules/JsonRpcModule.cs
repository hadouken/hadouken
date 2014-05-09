using System.IO;
using System.ServiceModel;
using System.Text;
using Hadouken.Configuration;
using Hadouken.Fx.JsonRpc;
using Hadouken.Fx.ServiceModel;

namespace Hadouken.Http.Management.Modules
{
    public class JsonRpcModule : ModuleBase
    {
        public JsonRpcModule(IConfiguration configuration, IJsonRpcRequestParser requestParser)
        {
            Post["/jsonrpc"] = _ =>
            {
                using (var ms = new MemoryStream())
                {
                    Request.Body.CopyTo(ms);
                    var json = Encoding.UTF8.GetString(ms.ToArray());

                    var request = requestParser.Parse(json);
                    var plugin = request.MethodName.Split('.')[0];
                    var binding = new NetNamedPipeBinding();
                    var endpoint = new EndpointAddress(string.Format(configuration.Rpc.PluginUriTemplate, plugin));

                    using (var factory = new ChannelFactory<IPluginService>(binding, endpoint))
                    {
                        var channel = factory.CreateChannel();
                        return channel.HandleJsonRpc(json);
                    }
                }
            };
        }
    }
}
