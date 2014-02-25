using System;
using Hadouken.Fx.JsonRpc;
using NUnit.Framework;

namespace Hadouken.Fx.Tests.JsonRpc
{
    [TestFixture]
    public class ParameterResolverTests
    {
        [Test]
        public void Resolve_WithNoParams_ReturnsNull()
        {
            // Given
            var resolver = new ParameterResolver(new JsonSerializer());

            // When
            var result = resolver.Resolve(14, null);

            // Then
            Assert.IsNull(result);
        }

        [Test]
        public void Resolve_WithInt32_ReturnsInt32()
        {
            // Given
            var resolver = new ParameterResolver(new JsonSerializer());

            // When
            var result = resolver.Resolve(14, new[] {new ParameterFake("value", typeof (int))});

            // Then
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(typeof (int), result[0].GetType());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Resolve_WithTypeMismatch_ThrowsParameterResolveException()
        {
            // Given
            var resolver = new ParameterResolver(new JsonSerializer());

            // When, Then
            resolver.Resolve("test", new[] {new ParameterFake("value", typeof (int))});
        }

        [Test]
        public void Resolve_WithComplexObject_ReturnsCorrectResolution()
        {
            // Given
            var resolver = new ParameterResolver(new JsonSerializer());

            // When
            var result = resolver.Resolve(new TestDto("foo", 1, 2, 3),
                new[] {new ParameterFake("value", typeof (TestDto))});

            // Then
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(typeof (TestDto), result[0].GetType());
        }

        [Test]
        public void Resolve_WithMultipleParameters_ReturnsCorrectResolution()
        {
            // Given
            var resolver = new ParameterResolver(new JsonSerializer());

            // When
            var result = resolver.Resolve(new object[] {12, "foo"},
                new[] {new ParameterFake("int", typeof (int)),
                    new ParameterFake("string", typeof (string))});

            // Then
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(typeof (int), result[0].GetType());
            Assert.AreEqual(typeof (string), result[1].GetType());
        }

        [Test]
        public void Resolve_WithMultipleParametersBothSimpleAndComplex_ReturnsCorrect()
        {
            // Given
            var resolver = new ParameterResolver(new JsonSerializer());

            // When
            var result = resolver.Resolve(new object[] {13, new TestDto("asdf", 1, 2), "foo"},
                new[]
                {
                    new ParameterFake("int", typeof (int)),
                    new ParameterFake("complex", typeof (TestDto)),
                    new ParameterFake("string", typeof (string))
                });

            // Then
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(typeof (int), result[0].GetType());
            Assert.AreEqual(typeof(TestDto), result[1].GetType());
            Assert.AreEqual(typeof (string), result[2].GetType());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Resolve_WithMismatchInParameterCount1_ThrowsException()
        {
            // Given
            var resolver = new ParameterResolver(new JsonSerializer());

            // When, Then
            resolver.Resolve(new[] {1, 2, 3}, new[] {new ParameterFake("value", typeof (int))});
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Resolve_WithMismatchInParameterCount2_ThrowsException()
        {
            // Given
            var resolver = new ParameterResolver(new JsonSerializer());

            // When, Then
            resolver.Resolve(new[] {1, 2, 3},
                new[] {
                    new ParameterFake("value", typeof (int)),
                    new ParameterFake("value2", typeof (string))
                });
        }

        private class TestDto
        {
            public TestDto()
            {
            }

            public TestDto(string foo, params int[] values)
            {
                Foo = foo;
                Ints = values;
            }
            public string Foo { get; set; }

            public int[] Ints { get; set; }
        }
    }

    public class ParameterFake : IParameter
    {
        public ParameterFake(string name, Type type)
        {
            Name = name;
            ParameterType = type;
        }
        public string Name { get; private set; }
        public Type ParameterType { get; private set; }
    }
}
