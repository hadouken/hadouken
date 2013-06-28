using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Configuration;
using Hadouken.Http;
using Hadouken.Http.Api;
using Hadouken.Security;
using Moq;
using NUnit.Framework;

namespace Hadouken.UnitTests.Http.Api
{
    public class SetCredentialsTests
    {
        [Test]
        public void Wrong_password_returns_false()
        {
            var context = HttpContextHelper.CreateMockWithPostData(new
                {
                    oldPassword = "invalid-password"
                });

            var keyValueStore = new Mock<IKeyValueStore>();
            keyValueStore.Setup(k => k.Get<string>("auth.password")).Returns(Hash.Generate("hdkn"));

            var action = new SetCredentials(keyValueStore.Object);
            action.Context = context.Object;

            var result = action.Execute() as JsonResult;
            var success = (bool)result.Data;

            Assert.IsTrue(!success);
        }

        [Test]
        public void Can_change_password()
        {
            var context = HttpContextHelper.CreateMockWithPostData(new
            {
                oldPassword = "hdkn",
                password = "new-password"
            });

            var keyValueStore = new Mock<IKeyValueStore>();
            keyValueStore.Setup(k => k.Get<string>("auth.password")).Returns(Hash.Generate("hdkn"));

            var action = new SetCredentials(keyValueStore.Object);
            action.Context = context.Object;
            action.Execute();

            keyValueStore.Verify(k => k.Set("auth.password", It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Can_change_username()
        {
            var context = HttpContextHelper.CreateMockWithPostData(new
            {
                oldPassword = "hdkn",
                username = "new-username"
            });

            var keyValueStore = new Mock<IKeyValueStore>();
            keyValueStore.Setup(k => k.Get<string>("auth.password")).Returns(Hash.Generate("hdkn"));

            var action = new SetCredentials(keyValueStore.Object);
            action.Context = context.Object;
            action.Execute();

            keyValueStore.Verify(k => k.Set("auth.username", It.IsAny<string>()), Times.Once());
        }
    }
}
