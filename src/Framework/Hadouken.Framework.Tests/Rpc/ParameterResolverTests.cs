using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Hadouken.Framework.Tests.Rpc
{
    public class InvokerHarness
    {
        public void NoParams() {}

        public void SimpleParameter(string param) {}

        public void UnboundedParams(params int[] integers) {}

        public void UnboundedParams2(string param1, DateTime param2, params int[] param3) {}

        public void MultiParams(string param1, int param2) {}
    }

    public class ParameterResolverTests
    {
        [Test]
        public void NullParameterWithMatchingMethodSucceeds()
        {
            var invoker = new MethodInvoker(new InvokerHarness(), new InvokerHarness().GetType().GetMethod("NoParams"));
            var resolver = new ParameterResolver();

            var result = resolver.Resolve(null, invoker);

            Assert.IsNull(result);
        }

        [Test]
        public void SimpleParameterWithMatchingMethodSucceeds()
        {
            var invoker = new MethodInvoker(new InvokerHarness(),
                new InvokerHarness().GetType().GetMethod("SimpleParameter"));
            var resolver = new ParameterResolver();

            var result = resolver.Resolve(new JValue("data"), invoker);

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("data", result[0]);
        }

        [Test]
        public void SimpleParameterWithWrongTypeThrows()
        {
            var invoker = new MethodInvoker(new InvokerHarness(),
                new InvokerHarness().GetType().GetMethod("SimpleParameter"));
            var resolver = new ParameterResolver();

            var input = new JObject {{"foo", new JValue("bar")}};

            Assert.Throws<InvalidParametersException>(() => resolver.Resolve(input, invoker));
        }

        [Test]
        public void UnboundedParamsResolvesCorrectly()
        {
            var invoker = new MethodInvoker(new InvokerHarness(),
                new InvokerHarness().GetType().GetMethod("UnboundedParams"));
            var resolver = new ParameterResolver();

            var result = resolver.Resolve(new JArray(1, 2, 3, 4, 5), invoker);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void ComplexUnboundedParamsResolvesCorrectly()
        {
            var invoker = new MethodInvoker(new InvokerHarness(),
                new InvokerHarness().GetType().GetMethod("UnboundedParams2"));
            var resolver = new ParameterResolver();

            var result = resolver.Resolve(
                new JArray(
                    new JValue("param1"),
                    new JValue(new DateTime(1, 1, 1)),
                    new JArray(1, 2, 3, 4, 5)),
                invoker);

            Assert.AreEqual(3, result.Length);
        }

        [Test]
        public void MultipleParamsResolvesCorrectly()
        {
            var invoker = new MethodInvoker(new InvokerHarness(), typeof (InvokerHarness).GetMethod("MultiParams"));
            var resolver = new ParameterResolver();

            var result = resolver.Resolve(new JArray("value1", 2), invoker);

            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void MultipleNamedParamsResolvesCorrectly()
        {
            var invoker = new MethodInvoker(new InvokerHarness(), typeof(InvokerHarness).GetMethod("MultiParams"));
            var resolver = new ParameterResolver();

            var input = new JObject
            {
                {"param1", "value1"},
                {"param2", 2}
            };

            var result = resolver.Resolve(input, invoker);

            Assert.AreEqual(2, result.Length);
        }
    }
}
