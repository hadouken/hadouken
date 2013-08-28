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
    public class PluginsListTests
    {
        [Test]
        public void Execute_WithNoPlugins_ReturnsEmptyList()
        {
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.GetAll()).Returns(new IPluginManager[] {});

            var pluginsList = new PluginsList(engine.Object);
            var result = pluginsList.Execute();

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Execute_WithPlugins_ReturnsList()
        {
            var plugin = new Mock<IPluginManager>();
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.GetAll()).Returns(new[] {plugin.Object});

            var pluginsList = new PluginsList(engine.Object);
            var result = pluginsList.Execute();

            Assert.AreEqual(1, result.Length);
        }
    }
}
