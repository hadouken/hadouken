using System;
using Autofac;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Security.Cookies;
using Nancy.Owin;
using Owin;

namespace Hadouken.Http.Management
{
    public class HttpBackendServer : IHttpBackendServer
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly Microsoft.Owin.Host.HttpListener.OwinHttpListener listener__;

        private IDisposable _httpServer;

        public HttpBackendServer(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public void Start()
        {
            var opts = new StartOptions();
            opts.Urls.Add("http://localhost:7891/");
            opts.Urls.Add("http://192.168.0.21:7891/");

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
            builder.MapSignalR();

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