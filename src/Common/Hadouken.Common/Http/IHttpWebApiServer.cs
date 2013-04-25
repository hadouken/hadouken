using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http
{
    public interface IHttpWebApiServer
    {
        void Start();
        void Stop();
    }
}
