using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;

namespace Hadouken.Plugins.Events
{
    public class EventServer : IEventServer
    {
#pragma warning disable 0169
        private static readonly Microsoft.Owin.Host.HttpListener.OwinHttpListener __ref__;
#pragma warning restore 0169

        private readonly string _listenUri;
        private IDisposable _server;

        public EventServer(string listenUri)
        {
            _listenUri = listenUri;
        }

        public void Start()
        {
            _server = WebApp.Start<SignalRStartup>(_listenUri);
        }

        public void Stop()
        {
            if (_server != null)
                _server.Dispose();
        }
    }
}
