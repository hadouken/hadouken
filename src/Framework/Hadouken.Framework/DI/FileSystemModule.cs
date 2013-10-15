using Autofac;
using Hadouken.Framework.IO;

namespace Hadouken.Framework.DI
{
    public class FileSystemModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystem>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
