using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public class RedirectResult : ActionResult
    {
        private readonly string _url;

        public RedirectResult(string url)
        {
            _url = url;
        }

        public override void Execute(IHttpContext context)
        {
            context.Response.Redirect(_url);
        }
    }
}
