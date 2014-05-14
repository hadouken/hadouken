using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Web.Script.Serialization;

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
            var pluginId = args[0];
            var parentId = Convert.ToInt32(args[1]);

            var config = ReadConfig(pluginId + ".config");
            var host = PluginHost.Create(pluginId, config);

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

        private IDictionary<string, object> ReadConfig(string fileName)
        {
            using (var mmf = MemoryMappedFile.OpenExisting(fileName))
            using (var stream = mmf.CreateViewStream())
            using (var reader = new BinaryReader(stream))
            {
                var json = reader.ReadString();
                return new JavaScriptSerializer().Deserialize<IDictionary<string, object>>(json);
            }
        }
    }
}
