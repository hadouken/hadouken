using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Hadouken.Http
{
    public interface IHttpPostedFile
    {
        int ContentLength { get; }
        string ContentType { get; }
        string FileName { get; }
        Stream InputStream { get; }
    }
}
