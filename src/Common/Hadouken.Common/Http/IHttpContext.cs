using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Hadouken.Common.Http
{
    public interface IHttpContext
    {
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }
        IPrincipal User { get; }
    }
}
