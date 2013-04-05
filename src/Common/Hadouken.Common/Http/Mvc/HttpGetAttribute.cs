using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        public HttpGetAttribute(string route) : base(route, "GET") {}
    }
}
