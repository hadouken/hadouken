namespace Hadouken.Plugins.HttpJsonRpc
{
    public interface IHttpJsonRpcServer
    {
        void Open();
        void Close();

        void SetCredentials(string userName, string password);
    }
}