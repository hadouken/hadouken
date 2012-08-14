using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Hosting;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace Hadouken.Hosts.CommandLine
{
    public class Program
    {
        public static void Main(string[] args)
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

            var host = Kernel.Get<IHost>();
            host.Load();

            Console.ReadLine();

            host.Unload();
        }
    }
}
