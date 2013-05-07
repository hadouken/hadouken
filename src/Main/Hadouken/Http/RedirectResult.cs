using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public class RedirectResult : ActionResult
    {
        private string _url;

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
