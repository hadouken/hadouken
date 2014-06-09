using System;
using Autofac;
using Hadouken.Configuration;
using NuGet;

namespace Hadouken.DI.Modules
{
    public sealed class NuGetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // NuGet Package Repository
            builder.Register(c =>
            {
                var cfg = c.Resolve<IConfiguration>();
                return PackageRepositoryFactory.Default.CreateRepository(cfg.PluginRepositoryUrl);
            });

            // NuGet Package Manager
            builder.Register<IPackageManager>(c =>
            {
                var cfg = c.Resolve<IConfiguration>();
                var repo = c.Resolve<IPackageRepository>();
                var expandedDirectory = Environment.ExpandEnvironmentVariables(cfg.PluginDirectory);

                return new PackageManager(repo, expandedDirectory);
            });
        }
    }
}
