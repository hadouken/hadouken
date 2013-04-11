using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public class HttpNotFoundResult : ActionResult
    {
        public override void Execute(IHttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
