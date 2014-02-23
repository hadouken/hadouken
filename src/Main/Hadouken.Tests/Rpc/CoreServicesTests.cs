using System;
using Hadouken.Configuration;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Security;
using Hadouken.Rpc;
using Moq;
using NUnit.Framework;

namespace Hadouken.Tests.Rpc
{
    public class CoreServicesTests
    {
        [Test]
        public void SetAuth_WithNoExistingCredentials_ReturnsTrue()
        {
            // Given
            var configuration = CreateConfiguration();
            var service = CreateCoreServices(configuration);

            // When
            var result = service.SetAuth("test", "test", "");

            // Then
            Assert.IsTrue(result);
        }

        [Test]
        public void SetAuth_WithFaultyUserName_ReturnsFalse()
        {
            // Given
            var configuration = CreateConfiguration("test", "testpass");
            var service = CreateCoreServices(configuration);

            // When
            var result = service.SetAuth("invalid username", "newpass", "testpass");

            // Then
            Assert.IsFalse(result);
        }

        [Test]
        public void SetAuth_WithFaultyExistingPassword_ReturnsFalse()
        {
            // Given
            var configuration = CreateConfiguration("test", "testpass");
            var service = CreateCoreServices(configuration);

            // When
            var result = service.SetAuth("test", "newpass", "invalid old password");

            // Then
            Assert.IsFalse(result);
        }

        private IConfiguration CreateConfiguration(string userName = null, string password = null)
        {
            var conf = new Mock<IConfiguration>();
            var httpConf = new Mock<IHttpConfiguration>();
            var httpAuthConf = new Mock<IHttpAuthConfiguration>();

            if (!string.IsNullOrEmpty(userName))
            {
                httpAuthConf.SetupGet(hac => hac.UserName).Returns(userName);                
            }

            if (!string.IsNullOrEmpty(password))
            {
                httpAuthConf.SetupGet(hac => hac.Password).Returns(HashProvider.GetDefault().ComputeHash(password));                
            }

            httpConf.SetupGet(hc => hc.Authentication).Returns(httpAuthConf.Object);
            conf.SetupGet(c => c.Http).Returns(httpConf.Object);

            return conf.Object;
        }

        public CoreServices CreateCoreServices(IConfiguration configuration = null, IJsonRpcClient rpcClient = null)
        {
            if (configuration == null)
            {
                configuration = new Mock<IConfiguration>().Object;
            }

            if (rpcClient == null)
            {
                rpcClient = new Mock<IJsonRpcClient>().Object;
            }

            return new CoreServices(configuration, rpcClient);
        }
    }
}
