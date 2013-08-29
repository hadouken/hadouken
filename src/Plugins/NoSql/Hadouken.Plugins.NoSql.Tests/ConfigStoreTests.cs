using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hadouken.Plugins.NoSql.Tests
{
    public class ConfigStoreTests
    {
        [Test]
        public void Get_WithNonExistentKey_ReturnsNull()
        {
            var store = new ConfigStore(":in-memory:");

            var value = store.Get("testkey");

            Assert.IsNull(value);
        }

        [Test]
        public void Get_WithExistingKey_ReturnsValue()
        {
            var store = new ConfigStore(":in-memory:");
            store.Set("key1", new {Foo = "Bar"});

            var value = store.Get("key1");

            Assert.IsNotNull(value);
        }
    }
}
