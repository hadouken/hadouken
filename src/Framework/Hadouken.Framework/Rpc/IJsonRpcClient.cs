using System;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface IJsonRpcClient : IDisposable
    {
        Task<TResult> CallAsync<TResult>(string method, object parameters = null);
    }

    public static class JsonRpcClientExtensions
    {
        public static Task SendEventAsync(this IJsonRpcClient client, string eventName, object data)
        {
            return client.CallAsync<bool>("events.publish", new
            {
                name = eventName,
                data
            });
        }
    }
}