using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Hadouken.Fx;
using Hadouken.Fx.Reflection;
using NLog;

namespace Hadouken.Plugins.Isolation
{
    public class AppDomainIsolatedEnvironment : IIsolatedEnvironment
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _baseDirectory;

        private Sandbox _sandbox;

        public AppDomainIsolatedEnvironment(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public void Load()
        {
            _sandbox = Sandbox.Create(_baseDirectory);
            _sandbox.Load();
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
        private IPluginHost _pluginHost;

        public static Sandbox Create(string applicationBase)
        {
            var rand = Path.GetRandomFileName();

            var setup = new AppDomainSetup()
            {
                ApplicationBase = applicationBase,
                ApplicationName = rand,
                ConfigurationFile = "", // DO not set to empty string if we want to use the conf file from this domain
                DisallowBindingRedirects = true,
                DisallowCodeDownload = true,
                DisallowPublisherPolicy = true
            };

            var permissions = new PermissionSet(PermissionState.Unrestricted);

            var domain = AppDomain.CreateDomain(rand, null, setup, permissions);
            return (Sandbox)Activator.CreateInstanceFrom(domain, typeof(Sandbox).Assembly.ManifestModule.FullyQualifiedName, typeof(Sandbox).FullName).Unwrap();
        }

        public void Load()
        {
            var retriever = new AssemblyNameRetriever();
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var assemblyNames = retriever.GetAssemblyNames(files);

            var assembly = Assembly.LoadFile(assemblyNames.First().CodeBase.Replace("file:///", ""));

            Type type = (from t in assembly.GetTypes()
                         where t.IsClass && !t.IsAbstract
                         where typeof(Plugin).IsAssignableFrom(t)
                         select t).FirstOrDefault();

            if (type == null)
                return;

            // Find bootstrapper
            var bootstrapperAttribute = type.GetCustomAttribute<PluginBootstrapperAttribute>();
            
            // Create and initialize bootstrapper
            var bootstrapper = (IBootstrapper) Activator.CreateInstance(bootstrapperAttribute.Type);
            bootstrapper.Initialize();

            // Use it to get the IPluginHost
            _pluginHost = bootstrapper.GetHost();
            _pluginHost.Load();
        }

        public void Unload()
        {
            _pluginHost.Unload();
        }

        public AppDomain GetAppDomain()
        {
            return AppDomain.CurrentDomain;
        }
    }
}