namespace Hadouken.Fx.JsonRpc
{
    public interface IJsonRpcClient
    {
        object Call(string method, object parameters);

        object Call(string method);

        T Call<T>(string method);

        T Call<T>(string method, object parameters);
    }
}
