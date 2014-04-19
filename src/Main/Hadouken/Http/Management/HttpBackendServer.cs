using System;
using Autofac;
using Hadouken.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Security.Cookies;
using Nancy.Owin;
using Owin;

namespace Hadouken.Http.Management
{
    public class HttpBackendServer : IHttpBackendServer
    {
#pragma warning disable 169
        private readonly Microsoft.Owin.Host.HttpListener.OwinHttpListener listener__;
#pragma warning restore 169

        private readonly IConfiguration _configuration;
        private readonly ILifetimeScope _lifetimeScope;

        private IDisposable _httpServer;

        public HttpBackendServer(IConfiguration configuration, ILifetimeScope lifetimeScope)
        {
            _configuration = configuration;
            _lifetimeScope = lifetimeScope;
        }

        public void Start()
        {
            var opts = new StartOptions();
            opts.Urls.Add(string.Format("http://{0}:{1}/", _configuration.Http.HostBinding, _configuration.Http.Port));

            _httpServer = WebApp.Start(opts, BuildApplication);
        }

        public void Stop()
        {
            _httpServer.Dispose();
        }

        private void BuildApplication(IAppBuilder builder)
        {
            // Cookie auth
            var cookieOpts = new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                CookieName = "_hdkn_auth",
                LoginPath = new PathString("/login"),
                LogoutPath = new PathString("/logout")
            };

            builder.UseCookieAuthentication(cookieOpts);

            // SignalR
            builder.MapSignalR(new HubConfiguration {EnableDetailedErrors = true});

            // Nancy
            var nancyOpts = new NancyOptions
            {
                Bootstrapper = new CustomNancyBootstrapper(_lifetimeScope),
                PerformPassThrough = context => context.Request.Path == "/signalr"
            };

            builder.UseNancy(nancyOpts);
        }
    }
}