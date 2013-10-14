using Hadouken.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using NLog;
using Owin;
using System;
using System.Threading.Tasks;

namespace Hadouken.Events
{
    public class EventServer : IEventServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;
#pragma warning disable 0169
        private readonly Microsoft.Owin.Host.HttpListener.OwinHttpListener _factory;
#pragma warning restore 0169

        private IDisposable _signalR;

        public EventServer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Open()
        {
            string host = _configuration.Http.HostBinding;
            int port = _configuration.Http.Port;
            string url = String.Format("http://{0}:{1}/events", host, port);

            Logger.Info("Opening event server on {0}", url);

            _signalR = WebApp.Start<Startup>(url);
        }

        public void Close()
        {
            _signalR.Dispose();
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // TODO: lockdown origin

            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.MapSignalR();
        }
    }

    [HubName("events")]
    public class EventsHub : Hub
    {
    }
}
