using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;
using NUnit.Framework;

namespace Hadouken.Plugins.Events.Tests
{
    public class EventsBootstrapperTests
    {
        [Test]
        public void Load_ReturnsPluginInstance()
        {
            // Arrange
            var boot = new EventsBootstrapper();

            // Act
            var plugin = boot.Load(new BootConfig {HostBinding = "http://test"});

            // Assert
            Assert.IsNotNull(plugin);
        }
    }
}
