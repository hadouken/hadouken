using Hadouken.Plugins;
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
        }

        [Test]
        public void Execute_WithValidPluginName_CallsLoad()
        {
        }
    }
}
