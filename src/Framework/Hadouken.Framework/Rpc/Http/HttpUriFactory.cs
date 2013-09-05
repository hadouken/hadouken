namespace Hadouken.Framework.Rpc.Http
{
    public class HttpUriFactory : IHttpUriFactory
    {
        private readonly string _listenUri;

        public HttpUriFactory(string listenUri)
        {
            _listenUri = listenUri;
        }

        public string GetListenUri()
        {
            return _listenUri;
        }
    }
}