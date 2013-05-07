using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Hadouken.Http
{
    public abstract class ActionResult
    {
        public abstract void Execute(IHttpContext context);
    }
}
