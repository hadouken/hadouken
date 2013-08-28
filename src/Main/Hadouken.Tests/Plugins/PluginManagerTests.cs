using Hadouken.Plugins;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Tests.Plugins
{
    public class PluginManagerTests
    {
        [Test]
        public void Load_WithValidPluginPath_CreatesSandbox()
        {
            var manager =
                new PluginManager(
                    @"../../../../Plugins/Events/Hadouken.Plugin.Events/bin/Debug");

            Assert.DoesNotThrow(() =>
                {
                    manager.Load();
                    manager.Unload();
                });
        }
    }
}
