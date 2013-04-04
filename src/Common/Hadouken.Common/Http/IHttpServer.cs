using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Hadouken.Common.Http
{
    public interface IHttpServer
    {
        void Start(Uri binding, NetworkCredential credential);
        void Stop();

        void SetRequestCallback(Action<IHttpContext> requestCallback);
    }
}
