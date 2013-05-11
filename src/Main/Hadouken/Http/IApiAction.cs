using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public interface IApiAction
    {
        IHttpContext Context { get; set; }

        ActionResult Execute();
    }
}
