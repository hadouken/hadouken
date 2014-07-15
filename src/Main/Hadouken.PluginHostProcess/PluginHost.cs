using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Hosting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;

namespace Hadouken.PluginHostProcess
{
    public class PluginHost : MarshalByRefObject
    {
        private const string FxAssembly = "Hadouken.Fx.dll";
        private const string FxPlugin = "Hadouken.Fx.Plugin";
        private const string FxBootstrapperAttribute = "Hadouken.Fx.Bootstrapping.BootstrapperAttribute";
        private const string FxTinyIoCBootstrapper = "Hadouken.Fx.Bootstrapping.TinyIoC.TinyIoCBootstrapper";

        private readonly IDictionary<string, object> _configuration;
        private readonly EventWaitHandle _handle;

        private object _pluginHost;

        public PluginHost(string pluginId, IDictionary<string, object> configuration)
        {
            _configuration = configuration;
            _handle = EventWaitHandle.OpenExisting(pluginId);
        }

        public static PluginHost Create(string pluginId, IDictionary<string, object> config)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var setup = new AppDomainSetup
            {
                ApplicationBase = currentDirectory,
                ApplicationName = pluginId,
                ConfigurationFile = "", // DO not set to empty string if we want to use the conf file from this domain
                DisallowBindingRedirects = true,
                DisallowCodeDownload = true,
                DisallowPublisherPolicy = true
            };

            var permissions = new PermissionSet(PermissionState.None);

            if (config.ContainsKey("Permissions") && config["Permissions"] != null)
            {
                var securityElement = SecurityElement.FromString(config["Permissions"].ToString());

                if (securityElement != null)
                {
                    permissions.FromXml(securityElement);                    
                }
            }
            
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlEvidence // To get nice exceptions with permission demands.
                | SecurityPermissionFlag.ControlPolicy   // See ^
                | SecurityPermissionFlag.Execution       // To allow the plugin to execute
                ));


            // WCF hosting for JSONRPC
            permissions.AddPermission(new WebPermission(NetworkAccess.Connect | NetworkAccess.Accept,
                new Regex(@"http://localhost:31337/hadouken\.plugins.*")));

            // Isolated storage
            permissions.AddPermission(new IsolatedStorageFilePermission(PermissionState.Unrestricted));

            var ev = new Evidence(
                new EvidenceBase[] {new Url("hadouken://host")},
                new EvidenceBase[] {new Url(config["Url"].ToString())});

            var fxAsm = Assembly.LoadFile(Path.Combine(currentDirectory, FxAssembly));
            var domain = AppDomain.CreateDomain(pluginId, ev, setup, permissions,
                typeof (PluginHost).Assembly.Evidence.GetHostEvidence<StrongName>(),
                fxAsm.Evidence.GetHostEvidence<StrongName>());

            return (PluginHost) Activator.CreateInstanceFrom(
                domain,
                typeof (PluginHost).Assembly.ManifestModule.FullyQualifiedName,
                typeof (PluginHost).FullName,
                false,
                BindingFlags.Default,
                null,
                new object[] {pluginId, config},
                null,
                null).Unwrap();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Load(string path)
        {
            new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, path).Assert();
            
            var files = Directory.GetFiles(".", "*.dll");
            foreach (var file in files)
            {
                Assembly.LoadFrom(file);
            }

            CodeAccessPermission.RevertAssert();

            Type type = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from t in asm.GetTypes()
                where t.IsClass && !t.IsAbstract
                where t.BaseType != null && t.BaseType.FullName == FxPlugin
                select t).FirstOrDefault();

            if (type == null)
                return;

            var fxAssembly = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from t in asm.GetTypes()
                where t.FullName == FxPlugin
                select asm).Single();

            // Find bootstrapper
            var bootstrapperAttributeType = fxAssembly.GetType(FxBootstrapperAttribute, true, true);
            var bootstrapperAttribute = type.GetCustomAttribute(bootstrapperAttributeType);
            object bootstrapper;

            if (bootstrapperAttribute == null)
            {
                // Hard coded default bootstrapper
                var defaultBootstrapper = fxAssembly.GetType(FxTinyIoCBootstrapper);
                bootstrapper = Activator.CreateInstance(defaultBootstrapper);
            }
            else
            {
                // Use the plugin specified one
                var customBootstrapperType = bootstrapperAttribute.GetPropertyValue<Type>("Type");
                bootstrapper = Activator.CreateInstance(customBootstrapperType);
            }

            bootstrapper.Invoke("Initialize", new object[] {_configuration});

            // Use it to get the IPluginHost
            _pluginHost = bootstrapper.Invoke("GetHost");
            _pluginHost.Invoke("Load");

            _handle.Set();
        }

        public void Unload()
        {
            _pluginHost.Invoke("Unload");
            _handle.Set();
        }

        public void SetupMonitoring(int parentProcessId)
        {
            new SecurityPermission(PermissionState.Unrestricted).Assert();

            var parentProcess = Process.GetProcessById(parentProcessId);
            parentProcess.EnableRaisingEvents = true;
            parentProcess.Exited += (sender, eventArgs) => Environment.Exit(9);

            CodeAccessPermission.RevertAssert();
        }

        public void WaitForExit()
        {
            _handle.WaitOne();
        }
    }
}
