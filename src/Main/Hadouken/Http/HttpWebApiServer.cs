using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.SelfHost;

namespace Hadouken.Http
{
    public class HttpWebApiServer : IHttpWebApiServer
    {
        private readonly IConfiguration _configuration;
        private readonly IDependencyResolver _dependencyResolver;
        private HttpSelfHostServer _selfHostServer;

        public HttpWebApiServer(IConfiguration configuration, IDependencyResolver dependencyResolver)
        {
            _configuration = configuration;
            _dependencyResolver = dependencyResolver;
        }

        public Task OpenAsync()
        {
            var uri = String.Format("http://{0}:{1}/", _configuration.HostBinding, _configuration.Port);

            var cfg = new HttpSelfHostConfiguration(uri)
            {
                DependencyResolver = _dependencyResolver
            };

            cfg.Routes.MapHttpRoute("Default", "api/{controller}/{id}", new {id = RouteParameter.Optional});

            _selfHostServer = new HttpSelfHostServer(cfg);

            return _selfHostServer.OpenAsync();
        }

        public Task CloseAsync()
        {
            return _selfHostServer.CloseAsync();
        }
    }
}
