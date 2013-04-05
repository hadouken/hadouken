using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.HttpListener
{
    [Component]
    public class HttpListenerServerFactory : IHttpServerFactory
    {
        public IHttpServer Create(string binding, System.Net.NetworkCredential credential)
        {
            return new HttpListenerServer(new Uri(binding), credential);
        }
    }
}
