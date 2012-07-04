using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    [Component(ComponentLifestyle.Transient)]
    public interface IController : IComponent
    {
        IHttpContext Context { get; }
    }
}
