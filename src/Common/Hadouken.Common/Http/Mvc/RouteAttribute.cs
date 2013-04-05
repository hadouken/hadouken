using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public RouteAttribute(string route)
        {
            Route = route;
        }

        public string Route { get; private set; }
    }
}
