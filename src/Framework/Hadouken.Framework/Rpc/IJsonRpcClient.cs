using System;

namespace Hadouken.Framework.Rpc
{
    public interface IJsonRpcClient : IDisposable
    {
        TResult Call<TResult>(string method, object parameters = null);
    }

    public static class JsonRpcClientExtensions
    {
        public static void SendEvent(this IJsonRpcClient client, string eventName, object data = null)
        {
            client.Call<bool>("events.publish", new
            {
                name = eventName,
                data
            });
        }
    }
}