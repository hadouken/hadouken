using System;
using System.IO;

namespace Hadouken.PluginHostProcess
{
    public class Program
    {
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        public static void Main(string[] args)
        {
            NativeMethods.SetErrorMode(NativeMethods.SetErrorMode(0) |
                           ErrorModes.SEM_NOGPFAULTERRORBOX |
                           ErrorModes.SEM_FAILCRITICALERRORS |
                           ErrorModes.SEM_NOOPENFILEERRORBOX);

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Hadouken.PluginHostProcess.exe <id> <parent process id>");
                Environment.Exit(1);
            }

            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            // Get current process and create handler to close this child process if
            // parent exists. This is to prevent stray child processes.
            var pluginId = args[0];
            var parentId = Convert.ToInt32(args[1]);

            var host = PluginHost.Create(pluginId);

            // Setup host process monitoring. Kill our process
            // if the parent shuts down.
            host.SetupMonitoring(parentId);

            // Load our plugin.
            host.Load(Directory.GetCurrentDirectory());

            // Wait for the host to set the exit signal.
            host.WaitForExit();

            // Unload the plugin.
            host.Unload();
        }
    }
}
