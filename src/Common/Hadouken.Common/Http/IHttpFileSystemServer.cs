using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Hadouken.Common.Http
{
    public interface IHttpFileSystemServer
    {
        void Start();
        void Stop();
    }
}
