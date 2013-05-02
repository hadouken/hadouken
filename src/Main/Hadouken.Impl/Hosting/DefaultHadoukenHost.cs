using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Hadouken.Common.Data;
using Hadouken.Common.Http;
using Hadouken.Hosting;

using Hadouken.Data;
using Hadouken.Plugins;
using Hadouken.BitTorrent;
using NLog;
using Hadouken.Common;
using Hadouken.Configuration;

namespace Hadouken.Impl.Hosting
{
    [Component]
    public class DefaultHadoukenHost : IHadoukenHost
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEnvironment _environment;
        private readonly IBitTorrentEngine _torrentEngine;
        private readonly IKeyValueStore _keyValueStore;
        private readonly IMigrationRunner _migratorRunner;
        private readonly IPluginEngine _pluginEngine;

        private readonly IHttpServerFactory _serverFactory;
        private IHttpFileSystemServer _httpServer;
        private IHttpWebApiServer _webApiServer;

        public DefaultHadoukenHost(IEnvironment environment,
                                   IKeyValueStore keyValueStore,
                                   IBitTorrentEngine torrentEngine,
                                   IMigrationRunner runner,
                                   IPluginEngine pluginEngine,
                                   IHttpServerFactory httpServerFactory)
        {
            _environment = environment;
            _keyValueStore = keyValueStore;
            _torrentEngine = torrentEngine;
            _serverFactory = httpServerFactory;
            _migratorRunner = runner;
            _pluginEngine = pluginEngine;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.FatalException("Unhandled exception.", e.ExceptionObject as Exception);
        }

        public void Load()
        {
            Logger.Trace("Load()");

            Logger.Debug("Running migrations");
            _migratorRunner.Up(this.GetType().Assembly);

            Logger.Debug("Loading the IBitTorrentEngine implementation");
            _torrentEngine.Load();

            Logger.Debug("Loading the IPluginEngine implementation");
            _pluginEngine.Load();

            var httpUser = _keyValueStore.Get("http.auth.username", "hdkn");
            var httpPass = _keyValueStore.Get("http.auth.password", "hdkn");


            _httpServer = _serverFactory.Create(_environment.HttpBinding,
                                                   new NetworkCredential(httpUser, httpPass),
                                                   "C:\\temp\\webui");

            _webApiServer = _serverFactory.Create(_environment.HttpBinding + "api",
                                                     new NetworkCredential(httpUser, httpPass),
                                                     AppDomain.CurrentDomain.GetAssemblies());

            Logger.Debug("Starting the HTTP API server");
            _webApiServer.Start();
            
            Logger.Debug("Starting the HTTP UI server");
            _httpServer.Start();
        }

        public void Unload()
        {
            Logger.Trace("Unload()");

            _pluginEngine.UnloadAll();

            _torrentEngine.Unload();

            _httpServer.Stop();

            _webApiServer.Stop();
        }
    }
}
