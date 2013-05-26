using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Hosting;
using Hadouken.Http;
using Hadouken.Data;
using Hadouken.Plugins;
using Hadouken.BitTorrent;
using NLog;

namespace Hadouken.Impl.Hosting
{
    [Component]
    public class DefaultHost : IHost
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private IDataRepository _data;
        private IBitTorrentEngine _torrentEngine;
        private IMigrationRunner _migratorRunner;
        private IPluginEngine _pluginEngine;
        private IHttpServer _httpServer;

        public DefaultHost(IDataRepository data, IBitTorrentEngine torrentEngine, IMigrationRunner runner, IPluginEngine pluginEngine, IHttpServer httpServer)
        {
            _data = data;
            _torrentEngine = torrentEngine;
            _migratorRunner = runner;
            _pluginEngine = pluginEngine;
            _httpServer = httpServer;

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.FatalException("Unhandled exception.", e.ExceptionObject as Exception);
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var result = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                          where asm.GetName().FullName == args.Name
                          select asm).FirstOrDefault();

            return result;
        }

        public void Load()
        {
            _migratorRunner.Up(this.GetType().Assembly);

            _torrentEngine.Load();
            _pluginEngine.Load();

            _httpServer.Start();
        }

        public void Unload()
        {
            _pluginEngine.UnloadAll();

            _torrentEngine.Unload();

            _httpServer.Stop();
        }
    }
}
