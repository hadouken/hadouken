using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    [Component(ComponentLifestyle.Transient)]
    public interface IApiAction : IComponent
    {
        IHttpContext Context { get; set; }

        ActionResult Execute();
    }
}
