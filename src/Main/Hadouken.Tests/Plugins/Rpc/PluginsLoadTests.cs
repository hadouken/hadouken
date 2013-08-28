using Hadouken.Plugins;
using Hadouken.Plugins.Rpc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Tests.Plugins.Rpc
{
    public class PluginsLoadTests
    {
        [Test]
        public void Execute_WithInvalidPluginName_DoesNotCallLoad()
        {
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.Get(It.IsAny<string>())).Returns(() => null);

            var pluginsLoad = new PluginsLoad(engine.Object);

            Assert.DoesNotThrow(() => pluginsLoad.Execute("invalid-name"));
        }

        [Test]
        public void Execute_WithValidPluginName_CallsLoad()
        {
            var plugin = new Mock<IPluginManager>();
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.Get(It.IsAny<string>())).Returns(plugin.Object);

            var pluginsLoad = new PluginsLoad(engine.Object);
            pluginsLoad.Execute("something");

            Assert.DoesNotThrow(() => plugin.Verify(p => p.Load(), Times.Once()));
        }
    }
}
