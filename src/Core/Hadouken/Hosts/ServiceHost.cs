using System;
using System.ServiceProcess;
using System.Threading;
using Hadouken.Core;

namespace Hadouken.Hosts {
    public sealed class ServiceHost : ServiceBase {
        private readonly IHadoukenService _service;
        private readonly Thread _thread;
        private readonly ManualResetEvent _waitHandle;

        public ServiceHost(IHadoukenService service) {
            if (service == null) {
                throw new ArgumentNullException("service");
            }
            this._service = service;
            this._thread = new Thread(this.Run) {
                IsBackground = true,
                Name = "HadoukenService"
            };

            this._waitHandle = new ManualResetEvent(false);
        }

        protected override void OnStart(string[] args) {
            this._thread.Start(args);
        }

        protected override void OnStop() {
            this._waitHandle.Set();

            if (!this._thread.Join(5000)) {
                this._thread.Abort();
            }
        }

        private void Run(object args) {
            this._service.Load(args as string[]);
            this._waitHandle.WaitOne();
            this._service.Unload();
        }
    }
}