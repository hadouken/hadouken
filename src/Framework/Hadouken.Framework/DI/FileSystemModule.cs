using Autofac;
using Hadouken.Framework.IO.Local;

namespace Hadouken.Framework.DI
{
    public class FileSystemModule : ParameterlessConstructorModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LocalFileSystem>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
