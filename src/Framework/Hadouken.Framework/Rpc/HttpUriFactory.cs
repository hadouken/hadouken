namespace Hadouken.Framework.Rpc
{
    public class UriFactory : IUriFactory
    {
        private readonly string _listenUri;

        public UriFactory(string listenUri)
        {
            _listenUri = listenUri;
        }

        public string GetListenUri()
        {
            return _listenUri;
        }
    }
}