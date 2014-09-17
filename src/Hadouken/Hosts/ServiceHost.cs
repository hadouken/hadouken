using System;
using System.ServiceProcess;
using System.Threading;
using Hadouken.Core;

namespace Hadouken.Hosts
{
    public sealed class ServiceHost : ServiceBase
    {
        private readonly IHadoukenService _service;
        private readonly Thread _thread;
        private readonly ManualResetEvent _waitHandle;

        public ServiceHost(IHadoukenService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            _service = service;
            _thread = new Thread(Run)
            {
                IsBackground = true,
                Name = "HadoukenService"
            };

            _waitHandle = new ManualResetEvent(false);
        }

        protected override void OnStart(string[] args)
        {
            _thread.Start(args);
        }

        protected override void OnStop()
        {
            _waitHandle.Set();

            if (!_thread.Join(5000))
            {
                _thread.Abort();
            }
        }

        private void Run(object args)
        {
            _service.Load(args as string[]);
            _waitHandle.WaitOne();
            _service.Unload();
        }
    }
}
