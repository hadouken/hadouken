using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Common;
using Hadouken.Hosting;

namespace Hadouken.Hosts.WindowsService
{
    public class HdknService : ServiceBase
    {
        private IHadoukenHost _host;

        public HdknService()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.AutoLog = true;
            this.ServiceName = "Hadouken";
        }

        protected override void OnStart(string[] args)
        {
            Task.Factory.StartNew(() =>
                {
                    _host = Kernel.Get<IHadoukenHost>();
                    _host.Load();
                });
        }

        protected override void OnStop()
        {
            if (_host != null)
                _host.Unload();
        }
    }
}
