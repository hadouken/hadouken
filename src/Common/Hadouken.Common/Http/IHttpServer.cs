using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Hadouken.Common.Http
{
    public interface IHttpServer
    {
        FileLocationType FileLocationType { get; set; }
        string FileLocationBase { get; set; }

        void Start();
        void Stop();
    }
}
