using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Extensibility;
using Hadouken.Core.BitTorrent;

namespace Hadouken.Core
{
    public class Service : IService
    {
        private readonly ISessionHandler _sessionHandler;
        private readonly IEnumerable<IPlugin> _plugins;

        public Service(ISessionHandler sessionHandler, IEnumerable<IPlugin> plugins)
        {
            if (sessionHandler == null) throw new ArgumentNullException("sessionHandler");
            _sessionHandler = sessionHandler;
            _plugins = plugins ?? Enumerable.Empty<IPlugin>();
        }

        public void Load(string[] args)
        {
            _sessionHandler.Load();

            foreach (var plugin in _plugins)
            {
                plugin.Load();
            }
        }

        public void Unload()
        {
            foreach (var plugin in _plugins)
            {
                plugin.Unload();
            }

            _sessionHandler.Unload();
        }
    }
}