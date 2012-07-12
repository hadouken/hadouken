using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Hosting;
using System.IO;

namespace Hadouken.Hosts.CommandLine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // load assemblies into appdomain
            var impl = AppDomain.CurrentDomain.Load("Hadouken.Impl");
            var implBitTorrent = AppDomain.CurrentDomain.Load("Hadouken.Impl.BitTorrent");

            Kernel.Register(impl, implBitTorrent);

            var host = Kernel.Get<IHost>();
            host.Load();

            Console.ReadLine();

            host.Unload();
        }
    }
}
