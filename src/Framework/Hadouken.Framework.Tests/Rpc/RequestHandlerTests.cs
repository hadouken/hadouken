using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hadouken.Framework.Tests.Rpc
{
    public class RequestHandlerTests
    {
        [Test]
        public void Execute_WithValidRequest_ReturnsCorrectValue()
        {
        }

        [Test]
        public void Execute_WithMissingMethod_ReturnsMethodDoesNotExistError()
        {
        }

        [Test]
        public void Execute_WithNullRequest_ReturnsParseError()
        {
        }
    }
}
