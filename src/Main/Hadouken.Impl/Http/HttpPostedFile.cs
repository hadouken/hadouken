using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using System.IO;

namespace Hadouken.Impl.Http
{
    public class HttpPostedFile : IHttpPostedFile
    {
        public HttpPostedFile(int contentLength, string contentType, string fileName, Stream inputStream)
        {
            ContentLength = contentLength;
            ContentType = contentType;
            FileName = fileName;
            InputStream = inputStream;
        }

        public int ContentLength { get; private set; }
        public string ContentType { get; private set; }
        public string FileName { get; private set; }
        public System.IO.Stream InputStream { get; private set; }
    }
}
