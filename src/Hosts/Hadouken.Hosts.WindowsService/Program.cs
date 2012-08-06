using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Hadouken.Hosts.WindowsService
{
    public static class Program
    {
        public static void Main()
        {
            // load assemblies into appdomain
            var impl = AppDomain.CurrentDomain.Load("Hadouken.Impl");
            var implBitTorrent = AppDomain.CurrentDomain.Load("Hadouken.Impl.BitTorrent");

            // register base types
            Kernel.Register(impl, implBitTorrent);

            // run the service
            ServiceBase.Run(new ServiceBase[] { new HdknService() });
        }
    }
}
