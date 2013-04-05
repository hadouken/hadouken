using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class HttpMethodAttribute : Attribute
    {
        protected HttpMethodAttribute(string route, string method)
        {
            Route = route;
            Method = method;
        }

        public string Method { get; private set; }

        public string Route { get; private set; }
    }
}
