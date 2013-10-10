using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Hosting
{
    [ServiceContract]
    public interface IWcfRpcService
    {
        [OperationContract]
        string Call(string json);
    }
}
