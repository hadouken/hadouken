using System;
using Hadouken.Configuration;
using Hadouken.Fx.Security;
using Hadouken.JsonRpc;
using NSubstitute;
using Xunit;

namespace Hadouken.Tests.JsonRpc
{
    public class AuthServiceTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_If_Configuration_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(() => new AuthService(null));
            }
        }

        public class TheValidateMethod
        {
            [Fact]
            public void Should_Return_True_If_Username_And_Password_Match()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                conf.Http.Authentication.UserName.Returns("test");
                conf.Http.Authentication.Password.Returns(HashProvider.GetDefault().ComputeHash("test"));
                var service = new AuthService(conf);

                // When
                var result = service.Validate("test", "test");

                Assert.True(result);
            }

            [Fact]
            public void Should_Return_False_If_Username_Is_Incorrect()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                conf.Http.Authentication.UserName.Returns("test");
                conf.Http.Authentication.Password.Returns(HashProvider.GetDefault().ComputeHash("test"));
                var service = new AuthService(conf);

                // When
                var result = service.Validate("invalid", "test");

                Assert.False(result);
            }

            [Fact]
            public void Should_Return_False_If_Password_Is_Incorrect()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                conf.Http.Authentication.UserName.Returns("test");
                conf.Http.Authentication.Password.Returns(HashProvider.GetDefault().ComputeHash("test"));
                var service = new AuthService(conf);

                // When
                var result = service.Validate("test", "invalid");

                Assert.False(result);
            }
        }
    }
}
