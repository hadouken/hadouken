using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.Configuration;
using Hadouken.DI.Ninject;
using System.IO;

namespace Hadouken.Hosts.WindowsService
{
    public static class Program
    {
        public static void Main()
        {
            var startupPath = Path.GetDirectoryName(typeof (Program).Assembly.Location);

            foreach (string file in Directory.GetFiles(startupPath, "*.dll"))
            {
                Assembly.LoadFile(file);
            }

            // register base types
            Kernel.SetResolver(new NinjectDependencyResolver());

            if(Bootstrapper.RunAsConsoleIfRequested<HdknService>())
                return;

            // run the service
            ServiceBase.Run(new ServiceBase[] { new HdknService() });
        }
    }
}
