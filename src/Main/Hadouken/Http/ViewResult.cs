using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Hadouken.Configuration;

namespace Hadouken.Http
{
    public class ViewResult : ActionResult
    {
        private string _viewPath;

        public ViewResult(string viewPath)
        {
            _viewPath = viewPath;
        }

        public override void Execute(IHttpContext context)
        {
            string webUIRoot = HdknConfig.GetPath("Paths.WebUI");
            byte[] data = File.ReadAllBytes(Path.Combine(webUIRoot, "Views", _viewPath + ".html"));

            context.Response.ContentType = "text/html";
            context.Response.OutputStream.Write(data, 0, data.Length);
        }
    }
}
