namespace Hadouken.Configuration.AppConfig
{
    public class HttpAuthConfigurationWrapper : IHttpAuthConfiguration
    {
        private readonly HttpAuthConfiguration _authConfiguration;

        public HttpAuthConfigurationWrapper(HttpAuthConfiguration authConfiguration)
        {
            _authConfiguration = authConfiguration;
        }

        public string UserName
        {
            get { return _authConfiguration.UserName; }
            set { _authConfiguration.UserName = value; }
        }

        public string Password
        {
            get { return _authConfiguration.Password; }
            set { _authConfiguration.Password = value; }
        }
    }
}
