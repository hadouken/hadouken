using System;
using System.Linq;
using Autofac;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.DI.Modules
{
    public sealed class JsonRpcServiceModule : Module
    {
        private static readonly Type ServiceType = typeof (IJsonRpcService);

        protected override void Load(ContainerBuilder builder)
        {
            var implementations = (from type in ThisAssembly.GetTypes()
                where type.IsClass && !type.IsAbstract
                where ServiceType.IsAssignableFrom(type)
                select type);

            foreach (var implementation in implementations)
            {
                builder.RegisterType(implementation).As(ServiceType);
            }
        }
    }
}
