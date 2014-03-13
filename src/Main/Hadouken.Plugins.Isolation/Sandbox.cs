using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Hadouken.Fx;
using Hadouken.Fx.Bootstrapping;
using Hadouken.Fx.Bootstrapping.TinyIoC;
using Hadouken.Fx.Reflection;

namespace Hadouken.Plugins.Isolation
{
    public class Sandbox : MarshalByRefObject
    {
        private IPluginHost _pluginHost;

        public static Sandbox Create(string applicationBase)
        {
            var configFile = Directory.GetFiles(applicationBase, "*.config").FirstOrDefault();
            var rand = Path.GetRandomFileName();

            var setup = new AppDomainSetup()
            {
                ApplicationBase = applicationBase,
                ApplicationName = rand,
                ConfigurationFile = configFile,
                DisallowBindingRedirects = false,
                DisallowCodeDownload = true,
                DisallowPublisherPolicy = true,
                ShadowCopyFiles = "true"
            };

            var permissions = new PermissionSet(PermissionState.Unrestricted);

            var domain = AppDomain.CreateDomain(rand, null, setup, permissions);
            return (Sandbox)Activator.CreateInstanceFrom(domain, typeof(Sandbox).Assembly.ManifestModule.FullyQualifiedName, typeof(Sandbox).FullName).Unwrap();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Load(PluginConfiguration configuration)
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
            var bootstrapperAttribute = type.GetCustomAttribute<BootstrapperAttribute>();
            IBootstrapper bootstrapper;

            if (bootstrapperAttribute == null)
            {
                // Hard coded default bootstrapper
                bootstrapper = new TinyIoCBootstrapper();
            }
            else
            {
                // Use the plugin specified one
                bootstrapper = (IBootstrapper)Activator.CreateInstance(bootstrapperAttribute.Type);                
            }
            
            // Create and initialize bootstrapper
            bootstrapper.Initialize(configuration);

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