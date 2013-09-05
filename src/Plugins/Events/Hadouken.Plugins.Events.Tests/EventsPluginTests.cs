using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Wcf;
using NUnit.Framework;

namespace Hadouken.Plugins.Events.Tests
{
    public class EventsPluginTests
    {
        [Test]
        public void Load_Opens_EventServer_And_Accepts_Rpc_Calls()
        {
            var bootstrapper = new EventsBootstrapper();
            var plugin = bootstrapper.Load(new BootConfig {HostBinding = "localhost", Port = 1234});
            plugin.Load();

            var rpcClient = new JsonRpcClient(new WcfClientTransport("net.pipe://localhost/hdkn.rpc.events"));
            var result = rpcClient.Call<bool>("events.publish", new {eventName = "foo", data = "bar"}).Result;

            plugin.Unload();

            Assert.IsTrue(result);
        }
    }
}
