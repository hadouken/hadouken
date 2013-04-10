using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public interface IControllerFactory
    {
        void Execute(IHttpContext context);
    }
}
