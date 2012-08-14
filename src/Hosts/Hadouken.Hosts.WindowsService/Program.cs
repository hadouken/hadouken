using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace Hadouken.Hosts.WindowsService
{
    public static class Program
    {
        public static void Main()
        {
            List<Assembly> assemblies = new List<Assembly>();

            foreach (string key in ConfigurationManager.AppSettings.Keys)
            {
                if (key.StartsWith("Assembly."))
                {
                    assemblies.Add(AppDomain.CurrentDomain.Load(ConfigurationManager.AppSettings[key]));
                }
            }

            // register base types
            Kernel.Register(assemblies.ToArray());

            // run the service
            ServiceBase.Run(new ServiceBase[] { new HdknService() });
        }
    }
}
