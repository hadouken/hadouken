using System;
using Autofac;
using Hadouken.Configuration;
using Nancy.Hosting.Self;

namespace Hadouken.Http.Management
{
    public class ManagementServer : IManagementServer
    {
        private readonly NancyHost _nancyHost;

        public ManagementServer(IConfiguration configuration, ILifetimeScope container)
        {
            var uri = new Uri(
                    string.Format("http://{0}:{1}/manage/",
                    configuration.Http.HostBinding,
                    configuration.Http.Port));

            var bootstrapper = new CustomNancyBootstrapper(container);
            _nancyHost = new NancyHost(bootstrapper, new HostConfiguration{RewriteLocalhost = false},new[] {uri});
        }

        public void Start()
        {
            _nancyHost.Start();
        }

        public void Stop()
        {
            _nancyHost.Stop();
        }
    }
}