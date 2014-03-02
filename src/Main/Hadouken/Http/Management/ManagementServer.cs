using System;
using Autofac;
using Hadouken.Configuration;
using Nancy.Hosting.Self;
using NLog;

namespace Hadouken.Http.Management
{
    public class ManagementServer : IManagementServer
    {
        private readonly IConfiguration _configuration;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly NancyHost _nancyHost;

        public ManagementServer(IConfiguration configuration, ILifetimeScope container)
        {
            _configuration = configuration;
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
            Logger.Debug("---");
            Logger.Info("Management server started on http://localhost:{0}/manage", _configuration.Http.Port);
            Logger.Debug("---");
        }

        public void Stop()
        {
            _nancyHost.Stop();
        }
    }
}