using Autofac;
using Hadouken.Framework.IO;

namespace Hadouken.Framework.DI
{
    public class FileSystemModule : ParameterlessConstructorModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystem>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
