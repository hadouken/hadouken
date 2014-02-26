using System;
using System.Linq;
using Hadouken.Fx.Bootstrapping.TinyIoC.WcfIntegration;
using Hadouken.Fx.IO;
using Hadouken.Fx.JsonRpc;
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
            // Register the plugin configuration to itself
            container.Register(Configuration);

            // Register the file system components
            container.Register<IFileSystem, FileSystem>();

            // Register plugin
            RegisterPlugin(container);

            // Register the plugin host
            container.Register<IPluginHost, PluginHost>();

            // Register IJsonRpcClient
            container.Register<IJsonSerializer, JsonSerializer>();
            container.Register<IJsonRpcClient, JsonRpcClient>();
            container.Register<IClientTransport>((tc, overloads) =>
            {
                var serializer = tc.Resolve<IJsonSerializer>();
                return new WcfClientTransport("net.pipe://localhost/hadouken.plugins.{0}", serializer);
            });

            // Register the JsonRpc server components
            container.Register<IJsonRpcRequestParser, JsonRpcRequestParser>();
            container.Register<IRequestHandler, RequestHandler>().AsSingleton();
            container.Register<IMethodCacheBuilder, MethodCacheBuilder>();
            container.Register<IParameterResolver, ParameterResolver>();

            // Register all IJsonRpcService implementations
            var jsonRpcServiceTypes = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from t in asm.GetTypes()
                where t.IsClass && !t.IsAbstract
                where typeof (IJsonRpcService).IsAssignableFrom(t)
                select t).ToArray();
            container.RegisterMultiple<IJsonRpcService>(jsonRpcServiceTypes);

            // Register IPluginServiceHost to host the WCF service
            container.Register<IPluginServiceHost>((tinyContainer, overloads) =>
            {
                var cfg = tinyContainer.Resolve<PluginConfiguration>();
                var uri = string.Format("net.pipe://localhost/hadouken.plugins.{0}", cfg.PluginName);

                var serviceHost = new TinyIoCServiceHost(
                    tinyContainer,
                    typeof (PluginService),
                    new Uri("net.pipe://localhost/hadouken.plugins.sample"));

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
