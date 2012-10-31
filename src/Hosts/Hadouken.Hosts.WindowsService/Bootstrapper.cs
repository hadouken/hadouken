using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace Hadouken.Hosts.WindowsService
{
    public static class Bootstrapper
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        public static bool RunAsConsoleIfRequested<T>() where T : ServiceBase, new()
        {
            if (!Environment.CommandLine.Contains("-console"))
                return false;

            var args = Environment.GetCommandLineArgs().Where(name => name != "-console").ToArray();

            AllocConsole();

            var service = new T();
            var onstart = service.GetType().GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onstart.Invoke(service, new object[] { args });

            Console.WriteLine("Your service named '" + service.GetType().FullName + "' is up and running.\r\nPress 'ENTER' to stop it.");
            Console.ReadLine();

            var onstop = service.GetType().GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onstop.Invoke(service, null);
            return true;
        }
    }
}
