using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Hadouken.Impl.Messaging;
using Hadouken.Messaging;

namespace Hadouken.UnitTests.Messaging
{
    public interface ITestMessage : IMessage
    {
        string Test { get; set; }
    }

    [TestFixture]
    public class MessageBusTests
    {
        [Test]
        public void Can_send_and_receive_messages()
        {
            bool seen = false;

            var mb = new DefaultMessageBus();
            mb.Subscribe<ITestMessage>(msg =>
            {
                seen = msg.Test == "1";
            });
            mb.Send<ITestMessage>(msgBuilder => { msgBuilder.Test = "1"; }).Wait();

            Assert.IsTrue(seen);
        }
    }
}
