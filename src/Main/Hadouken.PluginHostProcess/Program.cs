using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace Hadouken.PluginHostProcess
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Get current process and create handler to close this child process if
            // parent exists. This is to prevent stray child processes.
            var pluginId = args[0];
            var parentId = Convert.ToInt32(args[1]);

            var parentProcess = Process.GetProcessById(parentId);
            parentProcess.EnableRaisingEvents = true;
            parentProcess.Exited += (sender, eventArgs) => Environment.Exit(9);

            Console.WriteLine("Creating sandbox");
            var sandbox = new PluginHost();

            Console.WriteLine("Opening load event");
            using (var loadEvent = EventWaitHandle.OpenExisting(pluginId + ".load"))
            {
                Console.WriteLine("Loading sandbox");
                sandbox.Load(pluginId);
                loadEvent.Set();
            }

            // Wait until we're done here
            Console.WriteLine("Opening wait event");
            using (var waitEvent = EventWaitHandle.OpenExisting(pluginId + ".wait"))
            {
                waitEvent.WaitOne();
            }

            // Unload
            Console.WriteLine("Opening unload event");
            using (var unloadEvent = EventWaitHandle.OpenExisting(pluginId + ".unload"))
            {
                Console.WriteLine("Unloading sandbox");
                sandbox.Unload();
                unloadEvent.Set();
            }
        }
    }
}
