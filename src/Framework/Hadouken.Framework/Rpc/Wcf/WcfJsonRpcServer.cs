using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc.Wcf
{
    public class WcfJsonRpcServer : IJsonRpcServer
    {
        private readonly ServiceHost _serviceHost;
        private readonly IRequestBuilder _requestBuilder;
        private readonly IRequestHandler _requestHandler;

        public WcfJsonRpcServer(string address, IRequestBuilder requestBuilder, IRequestHandler requestHandler)
        {
            _requestBuilder = requestBuilder;
            _requestHandler = requestHandler;

            _serviceHost = new ServiceHost(new WcfRpcHost(HandleRequest));
            _serviceHost.AddServiceEndpoint(typeof (IWcfRpcHost), new NetNamedPipeBinding(),
                "net.pipe://localhost/" + address);
        }

        public void Start()
        {
            _serviceHost.Open();
        }

        public void Stop()
        {
            _serviceHost.Close();
        }

        private string HandleRequest(string jsonRequest)
        {
            var request = _requestBuilder.Build(jsonRequest);
            var response = _requestHandler.Execute(request);

            return JsonConvert.SerializeObject(response);
        }
    }
}
