using System;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface IJsonRpcClient : IDisposable
    {
        Task<TResult> CallAsync<TResult>(string method, object parameters = null);
    }
}