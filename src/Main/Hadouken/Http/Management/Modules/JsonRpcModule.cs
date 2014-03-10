using System.IO;
using System.ServiceModel;
using System.Text;
using Hadouken.Fx.JsonRpc;
using Hadouken.Fx.ServiceModel;
using Nancy;

namespace Hadouken.Http.Management.Modules
{
    public class JsonRpcModule : NancyModule
    {
        public JsonRpcModule(IJsonRpcRequestParser requestParser)
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
                    var endpoint = new EndpointAddress(string.Format("net.pipe://localhost/hadouken.plugins.{0}", plugin));

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
