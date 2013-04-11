using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public class ContentResult : ActionResult
    {
        public ContentResult(byte[] content, string mimeType)
        {
            Content = content;
            ContentType = mimeType;
        }

        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public override void Execute(IHttpContext context)
        {
            context.Response.ContentType = (ContentType ?? "text/plain");
            context.Response.OutputStream.Write(Content, 0, Content.Length);
        }
    }
}
