using System;
using Autofac;
using Hadouken.Common.Data;
using Hadouken.Common.Logging;
using Hadouken.Core.Http.WebSockets.Extensions;
using Hadouken.Core.Security;
using Microsoft.Owin.Hosting;
using Nancy.Bootstrapper;
using Owin;

namespace Hadouken.Core.Http
{
    public class HttpServer : IHttpServer
    {
#pragma warning disable 169
// ReSharper disable once InconsistentNaming
        private static readonly Microsoft.Owin.Host.HttpListener.OwinHttpListener _owinHttpListener;
#pragma warning restore 169

        private readonly ILogger<HttpServer> _logger;
        private readonly INancyBootstrapper _bootstrapper;
        private readonly IKeyValueStore _keyValueStore;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IUserManager _userManager;
        private IDisposable _owinHost;

        public HttpServer(ILogger<HttpServer> logger,
            INancyBootstrapper bootstrapper,
            IKeyValueStore keyValueStore,
            ILifetimeScope lifetimeScope,
            IUserManager userManager)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (bootstrapper == null) throw new ArgumentNullException("bootstrapper");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (lifetimeScope == null) throw new ArgumentNullException("lifetimeScope");
            if (userManager == null) throw new ArgumentNullException("userManager");
            _logger = logger;
            _bootstrapper = bootstrapper;
            _keyValueStore = keyValueStore;
            _lifetimeScope = lifetimeScope;
            _userManager = userManager;
        }

        public void Start()
        {
            var binding = _keyValueStore.Get("http.binding", "localhost");
            var port = _keyValueStore.Get("http.port", 7890);

            var startOptions = new StartOptions(string.Format("http://{0}:{1}/", binding, port));

            // Safeguard if some address is wrong
            if (binding != "localhost")
            {
                startOptions.Urls.Add("http://localhost:" + port + "/");
            }

            try
            {
                _owinHost = WebApp.Start(startOptions, AppBuilder);
                _logger.Info("HTTP server accepting connections on {Urls}.", startOptions.Urls);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Failed to start HTTP server.");
            }
        }

        public void Stop()
        {
            _owinHost.Dispose();
        }

        private void AppBuilder(IAppBuilder builder)
        {
            builder
                .MapWebSocketRoute<EventStreamServer>("/events", _lifetimeScope, _userManager)
                .UseNancy(nancyOptions => nancyOptions.Bootstrapper = _bootstrapper);
        }
    }
}