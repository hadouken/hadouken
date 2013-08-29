using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Transports
{
    public class JsonRpcServer : IJsonRpcServer
    {
        private readonly ITransport _transport;
        private readonly IRequestBuilder _requestBuilder;
        private readonly IEnumerable<IRpcMethod> _rpcMethods;

        public JsonRpcServer(ITransport transport,
                             IRequestBuilder requestBuilder,
                             IEnumerable<IRpcMethod> rpcMethods)
        {
            _transport = transport;
            _requestBuilder = requestBuilder;
            _rpcMethods = rpcMethods;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
