using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Extensibility;
using Hadouken.Core.BitTorrent;
using Hadouken.Core.Http;

namespace Hadouken.Core
{
    public class Service : IService
    {
        private readonly ISessionHandler _sessionHandler;
        private readonly IHttpServer _httpServer;
        private readonly IEnumerable<IPlugin> _plugins;

        public Service(ISessionHandler sessionHandler,
            IHttpServer httpServer,
            IEnumerable<IPlugin> plugins)
        {
            if (sessionHandler == null) throw new ArgumentNullException("sessionHandler");
            if (httpServer == null) throw new ArgumentNullException("httpServer");

            _sessionHandler = sessionHandler;
            _httpServer = httpServer;
            _plugins = plugins ?? Enumerable.Empty<IPlugin>();
        }

        public void Load(string[] args)
        {
            _sessionHandler.Load();

            foreach (var plugin in _plugins)
            {
                plugin.Load();
            }

            _httpServer.Start();
        }

        public void Unload()
        {
            _httpServer.Stop();

            foreach (var plugin in _plugins)
            {
                plugin.Unload();
            }

            _sessionHandler.Unload();
        }
    }
}