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
            var workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            foreach (var file in Directory.GetFiles(workingDirectory, "*.dll"))
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
