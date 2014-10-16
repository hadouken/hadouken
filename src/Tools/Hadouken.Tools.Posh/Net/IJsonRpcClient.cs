using System;

namespace Hadouken.Tools.Posh.Net
{
    public interface IJsonRpcClient
    {
        string AccessToken { get; set; }

        Uri Url { get; set; }

        void Call(string methodName, params object[] parameters);

        T Call<T>(string methodName, params object[] parameters);
    }
}
