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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main()
        {
            var startupPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Logger.Info("Starting Hadouken in {0}", startupPath);

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
