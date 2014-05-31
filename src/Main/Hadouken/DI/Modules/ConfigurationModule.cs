using System;
using Autofac;
using Hadouken.Configuration;

namespace Hadouken.DI.Modules
{
    public sealed class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppConfigConfiguration>()
                .As<IConfiguration>()
                .WithParameter("configurationFile", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)
                .SingleInstance();
        }
    }
}
