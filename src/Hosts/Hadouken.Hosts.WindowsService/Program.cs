using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.Configuration;
using Hadouken.Common;
using Hadouken.Common.DI.Ninject;
using System.IO;
using NLog;

namespace Hadouken.Hosts.WindowsService
{
    public static class Program
    {
        public static void Main()
        {
            var startupPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            foreach (var file in Directory.GetFiles(startupPath, "*.dll"))
            {
                if (file.ToLowerInvariant().EndsWith("sqlite.interop.dll"))
                    continue;

                try
                {
                    Assembly.LoadFile(file);
                }
                catch (Exception)
                {
                    return;
                }
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
