using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public abstract class ActionResult
    {
        public abstract void Execute(IHttpContext context);
    }
}
