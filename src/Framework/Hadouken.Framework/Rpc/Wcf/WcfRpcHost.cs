using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Wcf
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class WcfRpcHost : IWcfRpcHost
    {
        private readonly Func<string, string> _callback;

        public WcfRpcHost(Func<string, string> callback)
        {
            _callback = callback;
        }

        public string Call(string json)
        {
            return _callback(json);
        }
    }
}
