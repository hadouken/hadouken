using Hadouken.Common.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Impl.Http
{
    public class HttpApiRequestHandler
    {
        public static void Handle(IHttpContext httpContext)
        {
            httpContext.Response.Redirect("http://www.google.se");
            httpContext.Response.Close();
        }
    }
}
