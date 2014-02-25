using System;
using System.IO;
using System.Linq;
using Hadouken.Fx.Bootstrapping.TinyIoC.WcfIntegration;
using Hadouken.Fx.IO;
using Hadouken.Fx.ServiceModel;

namespace Hadouken.Fx.Bootstrapping.TinyIoC
{
    public class TinyIoCBootstrapper : Bootstrapper<TinyIoCContainer>
    {
        protected override TinyIoCContainer CreateContainer()
        {
            return new TinyIoCContainer();
        }

        protected override void RegisterComponents(TinyIoCContainer container)
        {
            // Register the file system components
            container.Register<IFileSystem, FileSystem>();

            // Register plugin
            RegisterPlugin(container);

            // Register the plugin host
            container.Register<IPluginHost, PluginHost>();

            // Register IPluginServiceHost to host the WCF service
            container.Register<IPluginServiceHost>((tinyContainer, overloads) =>
            {
                var rand = Path.GetRandomFileName();

                var serviceHost = new TinyIoCServiceHost(
                    tinyContainer,
                    typeof (PluginService),
                    new Uri("net.pipe://localhost/test" + rand));

                return new PluginServiceHost(serviceHost);
            });
        }

        protected void RegisterPlugin(TinyIoCContainer container)
        {
            Type type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from t in assembly.GetTypes()
                         where t.IsClass && !t.IsAbstract
                         where typeof(Plugin).IsAssignableFrom(t)
                         select t).FirstOrDefault();

            if (type == null)
            {
                throw new Exception("Could not find a plugin");
            }

            container.Register(typeof (Plugin), type);
        }

        protected override IPluginHost GetHost(TinyIoCContainer container)
        {
            return container.Resolve<IPluginHost>();
        }
    }
}
