namespace Hadouken.Configuration.AppConfig
{
    public class HttpConfigurationWrapper : IHttpConfiguration
    {
        private readonly HttpConfiguration _configuration;
        private readonly HttpAuthConfigurationWrapper _authConfiguration;

        public HttpConfigurationWrapper(HttpConfiguration configuration)
        {
            _configuration = configuration;
            _authConfiguration = new HttpAuthConfigurationWrapper(configuration.Authentication);
        }

        public string HostBinding
        {
            get { return _configuration.HostBinding; }
            set { _configuration.HostBinding = value; }
        }

        public int Port
        {
            get { return _configuration.Port; }
            set { _configuration.Port = value; }
        }

        public IHttpAuthConfiguration Authentication
        {
            get { return _authConfiguration; }
        }
    }
}
