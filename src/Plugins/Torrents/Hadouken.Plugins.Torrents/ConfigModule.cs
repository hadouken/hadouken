using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Hadouken.Framework;

namespace Hadouken.Plugins.Torrents
{
    public class ConfigModule : Module
    {
        private readonly IBootConfig _config;

        public ConfigModule(IBootConfig config)
        {
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => _config);
        }
    }
}
