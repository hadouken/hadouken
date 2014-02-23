using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Plugins;
using Hadouken.Plugins.Metadata;
using Hadouken.Plugins.Rpc;
using Moq;
using NUnit.Framework;

namespace Hadouken.Tests.Plugins.Rpc
{
    public class PluginsServiceTests
    {
        [Test]
        public void Load_WithoutExceptions_ReturnsTrue()
        {
            // Given
            var engine = new Mock<IPluginEngine>();
            var service = new PluginsService(engine.Object);

            // When
            var result = service.Load("plugin");

            // Then
            Assert.IsTrue(result);
        }

        [Test]
        public void Load_WithExceptions_ReturnsFalse()
        {
            // Given
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.Load(It.IsAny<string>())).Throws(new Exception());
            var service = new PluginsService(engine.Object);

            // When
            var result = service.Load("plugin");

            // Then
            Assert.IsFalse(result);
        }

        [Test]
        public void Unload_WithoutExceptions_ReturnsTrue()
        {
            // Given
            var engine = new Mock<IPluginEngine>();
            var service = new PluginsService(engine.Object);

            // When
            var result = service.Unload("plugin");

            // Then
            Assert.IsTrue(result);
        }

        [Test]
        public void Unload_WithExceptions_ReturnsFalse()
        {
            // Given
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.Unload(It.IsAny<string>())).Throws(new Exception());
            var service = new PluginsService(engine.Object);

            // When
            var result = service.Unload("plugin");

            // Then
            Assert.IsFalse(result);
        }

        [Test]
        public void List_WithNullPlugins_ReturnsEmptyArray()
        {
            // Given
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.GetAll()).Returns((IEnumerable<IPluginManager>) null);
            var service = new PluginsService(engine.Object);

            // When
            var result = service.List();

            // Then
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void List_WithEmptyPlugins_ReturnsEmptyArray()
        {
            // Given
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.GetAll()).Returns(Enumerable.Empty<IPluginManager>());
            var service = new PluginsService(engine.Object);

            // When
            var result = service.List();

            // Then
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void List_WithPopulatedList_ReturnsFilledArray()
        {
            // Given
            var engine = new Mock<IPluginEngine>();
            engine.Setup(e => e.GetAll()).Returns(new[] {CreateEmptyPluginManager()});
            var service = new PluginsService(engine.Object);

            // When
            var result = service.List();

            // Then
            Assert.AreEqual(1, result.Length);
        }

        private IPluginManager CreateEmptyPluginManager()
        {
            var manager = new Mock<IPluginManager>();
            var package = new Mock<IPackage>();
            package.SetupGet(p => p.Manifest).Returns(new Mock<IManifest>().Object);
            manager.SetupGet(m => m.Package).Returns(package.Object);

            return manager.Object;
        }
    }
}
