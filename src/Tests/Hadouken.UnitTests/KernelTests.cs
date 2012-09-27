using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.DI.Ninject;
using NUnit.Framework;
using Hadouken.Hosting;

namespace Hadouken.UnitTests
{
    public class KernelTests
    {
        [Test]
        public void Can_register_and_resolve_components()
        {
            Kernel.SetResolver(new NinjectDependencyResolver());
            Kernel.Register(AppDomain.CurrentDomain.Load("Hadouken.Impl"), AppDomain.CurrentDomain.Load("Hadouken.Impl.BitTorrent"));

            var host = Kernel.Resolver.Get<IHost>();

            Assert.IsNotNull(host);
        }
    }
}
