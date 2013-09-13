using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.SelfHost;
using Hadouken.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hadouken.Http
{
    public class HttpWebApiServer : IHttpWebApiServer
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        private readonly IConfiguration _configuration;
        private readonly IDependencyResolver _dependencyResolver;
        private HttpSelfHostServer _selfHostServer;

        static HttpWebApiServer()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SerializerSettings.Converters.Add(new VersionConverter());
            SerializerSettings.Formatting = Formatting.Indented;
        }

        public HttpWebApiServer(IConfiguration configuration, IDependencyResolver dependencyResolver)
        {
            _configuration = configuration;
            _dependencyResolver = dependencyResolver;
        }

        public Task OpenAsync()
        {
            var uri = String.Format("http://{0}:{1}/api", _configuration.Http.HostBinding, _configuration.Http.Port);

            var cfg = new HttpSelfHostConfiguration(uri)
            {
                DependencyResolver = _dependencyResolver,
                HostNameComparisonMode = HostNameComparisonMode.Exact,
            };

            cfg.Formatters.Clear();
            cfg.Formatters.Add(new JsonMediaTypeFormatter() {SerializerSettings = SerializerSettings});

            cfg.Routes.MapHttpRoute("Default", "{controller}/{id}", new {id = RouteParameter.Optional});

            _selfHostServer = new HttpSelfHostServer(cfg);

            return _selfHostServer.OpenAsync();
        }

        public Task CloseAsync()
        {
            return _selfHostServer.CloseAsync();
        }
    }
}
