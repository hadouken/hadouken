using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface IJsonRpcClient
    {
        Task<TResult> Call<TResult>(string method, object parameters);
    }
}
