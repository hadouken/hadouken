using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Hadouken.Common.Http
{
    public interface IHttpServerFactory
    {
        IHttpServer Create(string binding, NetworkCredential credential);
    }
}
