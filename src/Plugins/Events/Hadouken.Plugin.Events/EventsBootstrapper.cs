using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;

namespace Hadouken.Plugins.Events
{
    public class EventsBootstrapper : Bootstrapper
    {
        public EventsBootstrapper(IConfig config) : base(config) {}

        public override void RegisterComponents(IDependencyResolver resolver)
        {
            var endpoint = String.Format("http://{0}:{1}/events", Config.HostBinding, Config.Port);

            resolver.Register<IEventServer>(() => new EventServer(endpoint));
        }
    }
}
