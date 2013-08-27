using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Plugins;

namespace Hadouken.Framework
{
    public abstract class Bootstrapper
    {
        private readonly IConfig _config;

        protected Bootstrapper(IConfig config)
        {
            _config = config;
        }

        protected IConfig Config
        {
            get { return _config; }
        }

        public abstract void RegisterComponents(IDependencyResolver resolver);

        public virtual void RegisterPlugin(IDependencyResolver resolver)
        {
            var pluginType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                              from type in asm.GetTypes()
                              where type.IsClass && !type.IsAbstract
                              where typeof (Plugin).IsAssignableFrom(type)
                              select type).FirstOrDefault();

            if (pluginType == null)
            {
                throw new Exception("No plugin found");
            }

            resolver.Register<Plugin>(pluginType);
        }
    }
}
