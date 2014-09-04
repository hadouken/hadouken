using System.Linq;
using System.Reflection;
using Autofac;
using Hadouken.Common.Extensibility;
using Hadouken.Common.IO;
using Hadouken.Common.JsonRpc;
using Hadouken.Common.Logging;
using Hadouken.Common.Reflection;

namespace Hadouken
{
    public static class AutofacContainerExtensions
    {
        public static void LoadExtensions(this IContainer container, DirectoryPath path)
        {
            var fileSystem = container.Resolve<IFileSystem>();
            var finder = container.Resolve<IAssemblyNameFinder>();
            var logger = container.Resolve<ILogger>();
            
            var files = fileSystem.GetDirectory(path)
                .GetFiles("*.dll", SearchScope.Current)
                .Select(f => f.Path.FullPath);

            // Search through the path for assemblies to register
            var assemblyNames = finder.GetAssemblyNames<IExtension>(files);

            // The container builder which later updates the provided container
            var builder = new ContainerBuilder();

            foreach (var assemblyName in assemblyNames)
            {
                var filePath = new FilePath(assemblyName.CodeBase);
                logger.Info("Loading extension {FileName}.", filePath.GetFilename());

                var assembly = Assembly.Load(assemblyName);

                // Find extensions, services and custom components
                var extensions = assembly.GetTypesAssignableFrom<IExtension>();
                var services = assembly.GetTypesAssignableFrom<IJsonRpcService>();
                var components = (from type in assembly.GetTypes()
                    where !type.IsAbstract
                    let cattr = type.GetCustomAttribute<ComponentAttribute>()
                    where cattr != null
                    select type);

                // Register extensions
                foreach (var extension in extensions)
                {
                    var attr = extension.GetCustomAttribute<ExtensionAttribute>();
                    if (attr == null) continue;

                    builder.RegisterType(extension)
                        .AsImplementedInterfaces()
                        .SingleInstance();
                }

                // Register JSONRPC services
                foreach (var service in services)
                {
                    builder.RegisterType(service)
                        .As<IJsonRpcService>()
                        .SingleInstance();
                }

                // Register custom components
                foreach (var component in components)
                {
                    var attr = component.GetCustomAttribute<ComponentAttribute>();
                    var registration = builder.RegisterType(component).AsImplementedInterfaces();

                    switch (attr.Lifestyle)
                    {
                        case ComponentLifestyle.Singleton:
                            registration.SingleInstance();
                            break;
                        case ComponentLifestyle.Transient:
                            registration.InstancePerDependency();
                            break;
                    }
                }
            }

            // Update the container
            builder.Update(container);
        }
    }
}
