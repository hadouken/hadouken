using System.IO;

namespace Hadouken.Http.HttpServer
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
