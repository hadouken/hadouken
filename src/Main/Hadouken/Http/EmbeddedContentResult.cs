using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Hadouken.Http
{
    public class EmbeddedContentResult : ActionResult
    {
        private Assembly _assembly;
        private string _resourceName;
        private string _contentType;

        public EmbeddedContentResult(Assembly assembly, string resourceName, string contentType)
        {
            _assembly = assembly;
            _resourceName = resourceName;
            _contentType = contentType;
        }

        public override void Execute(IHttpContext context)
        {
            using(var ms = new MemoryStream())
            using (var stream = _assembly.GetManifestResourceStream(_resourceName))
            {
                stream.CopyTo(ms);

                var res = new ContentResult();
                res.Content = ms.ToArray();
                res.ContentType = _contentType;

                res.Execute(context);
            }
        }
    }
}
