using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hadouken.Hosting;

namespace Hadouken.UnitTests
{
    public class KernelTests
    {
        [Test]
        public void Can_register_and_resolve_components()
        {
            Kernel.Register(AppDomain.CurrentDomain.Load("Hadouken.Impl"), AppDomain.CurrentDomain.Load("Hadouken.Impl.BitTorrent"));

            var host = Kernel.Get<IHost>();

            Assert.IsNotNull(host);
        }
    }
}
