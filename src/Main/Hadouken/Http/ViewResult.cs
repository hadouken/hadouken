using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
            byte[] data = File.ReadAllBytes("WebUI/" + _viewPath);

            context.Response.OutputStream.Write(data, 0, data.Length);
        }
    }
}
