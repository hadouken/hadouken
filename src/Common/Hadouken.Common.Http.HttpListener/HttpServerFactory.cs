using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.ServiceModel;
using System.Text;
using System.Web.Http;
using Hadouken.Common.Http.Mvc;
using System.Net;
using System.Reflection;
using System.Web.Http.SelfHost;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hadouken.Common.Http.HttpListener
{
    [Component]
    public class HttpServerFactory : IHttpServerFactory
    {
        public IHttpFileSystemServer Create(Uri baseAddress, NetworkCredential credential, string path)
        {
            return new HttpListenerServer(baseAddress, credential, path);
        }

        public IHttpWebApiServer Create(Uri baseAddress, NetworkCredential credential, Assembly[] assemblies)
        {
            var config = new HttpSelfHostConfiguration(baseAddress);

            // Set up authentication
            config.ClientCredentialType = HttpClientCredentialType.Basic;
            config.UserNamePasswordValidator = new IdentityValidator(credential.UserName, credential.Password);

            // Replace services
            config.Services.Replace(typeof (IAssembliesResolver), new LocalizedAssembliesResolver(assemblies));

            // Set up formatter
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add(new VersionConverter());
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Replace with json formatter only
            config.Formatters.Clear();
            config.Formatters.Add(formatter);

            // Map routes
            config.Routes.MapHttpRoute(
                "API default",
                "{controller}/{id}",
                new {id = RouteParameter.Optional}
            );

            return new SelfHostedWebApiServer(config);
        }
    }
}
