using System;

namespace Hadouken.PluginHostProcess
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

            var host = new PluginHost(pluginId);

            // Setup host process monitoring. Kill our process
            // if the parent shuts down.
            host.SetupMonitoring(parentId);

            // Load our plugin.
            host.Load();

            // Wait for the host to set the exit signal.
            host.WaitForExit();

            // Unload the plugin.
            host.Unload();
        }
    }
}
