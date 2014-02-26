namespace Hadouken.Fx.JsonRpc
{
    public interface IJsonRpcClient
    {
        T Call<T>(string method, object parameters);
    }
}
