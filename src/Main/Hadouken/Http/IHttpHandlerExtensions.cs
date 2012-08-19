using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public static class IHttpHandlerExtensions
    {
        public static ActionResult View(this ApiAction handler, string viewName)
        {
            return new ViewResult(viewName);
        }
    }
}
