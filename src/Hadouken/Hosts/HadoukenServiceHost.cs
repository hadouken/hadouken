using System;
using System.ServiceProcess;
using Hadouken.Core;

namespace Hadouken.Hosts
{
    public sealed class HadoukenServiceHost : ServiceBase
    {
        private readonly IHadoukenService _service;

        public HadoukenServiceHost(IHadoukenService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            _service = service;
        }

        protected override void OnStart(string[] args)
        {
            _service.Load(args);
        }

        protected override void OnStop()
        {
            _service.Unload();
        }
    }
}
