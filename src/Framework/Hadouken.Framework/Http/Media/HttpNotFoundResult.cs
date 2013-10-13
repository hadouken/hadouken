using System;
using System.Net;

namespace Hadouken.Framework.Http.Media
{
    public class HttpNotFoundResult : HandleResult
    {
        public override void Execute(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
        }
    }
}
