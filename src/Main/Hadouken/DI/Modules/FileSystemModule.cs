using Autofac;
using Hadouken.Fx.IO;

namespace Hadouken.DI.Modules
{
    public sealed class FileSystemModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            builder.RegisterType<RootPathProvider>().As<IRootPathProvider>().SingleInstance();
        }
    }
}
