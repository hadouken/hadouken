using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.IO;

namespace Hadouken.Framework.Http.Media
{
    public class ContentResult : HandleResult
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _contentType;
        private readonly string _path;

        public ContentResult(IFileSystem fileSystem, string contentType, string path)
        {
            _fileSystem = fileSystem;
            _contentType = contentType;
            _path = path;
        }

        public override void Execute(HttpListenerContext context)
        {
            context.Response.ContentType = _contentType;
            context.Response.StatusCode = 200;

            using (var reader = _fileSystem.OpenRead(_path))
            {
                reader.CopyTo(context.Response.OutputStream);
            }
        }
    }
}
