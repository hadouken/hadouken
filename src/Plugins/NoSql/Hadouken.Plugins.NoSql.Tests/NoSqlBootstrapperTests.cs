using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;
using NUnit.Framework;

namespace Hadouken.Plugins.NoSql.Tests
{
    public class NoSqlBootstrapperTests
    {
        [Test]
        public void Load_ReturnsPluginInstance()
        {
            var boot = new NoSqlBootstrapper();

            var plugin = boot.Load(new BootConfig());

            Assert.IsNotNull(plugin);
        }
    }
}
