using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;
using NLog;

namespace Hadouken.Plugins.Isolation
{
    public class AppDomainIsolatedEnvironment : IIsolatedEnvironment
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBootConfig _config;
        private Sandbox _sandbox;

        public AppDomainIsolatedEnvironment(IBootConfig config)
        {
            _config = config;
        }

        public void Load()
        {
            _sandbox = Sandbox.Create(_config);
            var path = Path.Combine(_config.ApplicationBasePath, _config.AssemblyFile);
            _sandbox.Load(path);
        }

        public void Unload()
        {
            _sandbox.Unload();

            var domain = _sandbox.GetAppDomain();
            AppDomain.Unload(domain);
            _sandbox = null;
        }

        public long GetMemoryUsage()
        {
            return -1;
        }
    }

    public class Sandbox : MarshalByRefObject
    {
        private Plugin _plugin;

        public static Sandbox Create(IBootConfig bootConfig)
        {
            var rand = Path.GetRandomFileName();

            var setup = new AppDomainSetup()
            {
                ApplicationBase = bootConfig.ApplicationBasePath,
                ApplicationName = rand,
                ConfigurationFile = "", // DO not set to empty string if we want to use the conf file from this domain
                DisallowBindingRedirects = true,
                DisallowCodeDownload = true,
                DisallowPublisherPolicy = true
            };

            var domain = AppDomain.CreateDomain(rand, null, setup);
            return (Sandbox)Activator.CreateInstanceFrom(domain, typeof(Sandbox).Assembly.ManifestModule.FullyQualifiedName, typeof(Sandbox).FullName).Unwrap();
        }

        public void Load(string assemblyFile)
        {
            var assembly = Assembly.LoadFile(assemblyFile);
            
            Type type = (from t in assembly.GetTypes()
                         where t.IsClass && !t.IsAbstract
                         where typeof(Plugin).IsAssignableFrom(t)
                         select t).FirstOrDefault();

            if (type == null)
                return;

            _plugin = Activator.CreateInstance(type) as Plugin;
            _plugin.OnStart();
        }

        public void Unload()
        {
            _plugin.OnStop();
        }

        public AppDomain GetAppDomain()
        {
            return AppDomain.CurrentDomain;
        }
    }
}