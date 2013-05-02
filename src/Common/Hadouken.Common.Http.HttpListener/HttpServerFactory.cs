using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.ServiceModel;
using System.Text;
using System.Web.Http;

using System.Net;
using System.Reflection;
using System.Web.Http.SelfHost;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Hadouken.Common.IO;

namespace Hadouken.Common.Http.HttpListener
{
    [Component]
    public class HttpServerFactory : IHttpServerFactory
    {
        private readonly IFileSystem _fileSystem;

        public HttpServerFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IHttpFileSystemServer Create(string baseAddress, NetworkCredential credential, string path)
        {
            return new HttpListenerServer(_fileSystem, baseAddress, credential, path);
        }

        public IHttpWebApiServer Create(string baseAddress, NetworkCredential credential, Assembly[] assemblies)
        {
            var config = new HttpSelfHostConfiguration(baseAddress.Replace("+", "localhost"));
            config.DependencyResolver = new KernelDependencyResolver();

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

            config.MaxReceivedMessageSize = 1048576;

            // Map routes
            config.Routes.MapHttpRoute(
                "API default",
                "{controller}/{id}",
                new {controller ="System", id = RouteParameter.Optional}
            );

            return new SelfHostedWebApiServer(config);
        }
    }
}
