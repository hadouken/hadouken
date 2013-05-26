using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Configuration;
using Hadouken.DI.Ninject;
using NUnit.Framework;
using Hadouken.Hosting;
using System.IO;

namespace Hadouken.UnitTests
{
    public class KernelTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            HdknConfig.ConfigManager = new MemoryConfigManager();
        }

        [Test]
        public void Can_register_and_resolve_components()
        {
            var path = Path.GetFullPath(".");

            Kernel.Bootstrap(path);
            Kernel.SetResolver(new NinjectDependencyResolver());
            
            var host = Kernel.Resolver.Get<IHost>();

            Assert.IsNotNull(host);
        }
    }
}
