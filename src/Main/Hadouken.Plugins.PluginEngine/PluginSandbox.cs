using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Hadouken.Common;
using Hadouken.Common.Plugins;
using Hadouken.Common.Messaging;
using Hadouken.Common.DI;
using Hadouken.Common.IO;
using NLog;

namespace Hadouken.Plugins.PluginEngine
{
    public class PluginSandbox : MarshalByRefObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Plugin _plugin;

        public PluginSandbox()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                    where asm.FullName == args.Name
                    select asm).FirstOrDefault();
        }

        public void AddAssemblies(IEnumerable<byte[]> assemblies)
        {
            foreach (var asm in assemblies)
            {
                AppDomain.CurrentDomain.Load(asm);
            }
        }

        public void Load(PluginManifest manifest)
        {
            Logger.Debug("Loading plugin '{0}' in the sandbox.", manifest.Name);

            var resolverType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                from type in asm.GetTypes()
                                where typeof (IDependencyResolver).IsAssignableFrom(type)
                                where type.IsClass && !type.IsAbstract
                                select type).First();

            var resolver = (IDependencyResolver) Activator.CreateInstance(resolverType);

            Kernel.SetResolver(resolver);
            Kernel.BindToFunc(() =>
                {
                    var factory = Kernel.Get<IMessageBusFactory>();
                    return factory.Create("hdkn.plugins." + manifest.Name.ToLowerInvariant());
                });

            Kernel.BindToFunc<IEnvironment>(() =>
                {
                    var databasePath = System.IO.Path.Combine(ConfigurationManager.AppSettings["Paths.Data"],
                                                              String.Format("hdkn.plugins.{0}.db", manifest.Name));

                    var env = new PluginEnvironment()
                        {
                            ConnectionString = String.Format("Data Source={0};", databasePath)
                        };

                    return env;
                });

            _plugin = Kernel.Get<Plugin>();
            _plugin.Load();
        }

        public void Unload()
        {
            _plugin.Unload();
        }
    }
}
