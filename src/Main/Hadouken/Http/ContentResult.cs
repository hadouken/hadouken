using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public class ContentResult : ActionResult
    {
        public string ContentType { get; set; }
        public byte[] Content { get; set; }

        public override void Execute(IHttpContext context)
        {
            context.Response.ContentType = ContentType;

            context.Response.OutputStream.Write(Content, 0, Content.Length);
        }
    }
}
