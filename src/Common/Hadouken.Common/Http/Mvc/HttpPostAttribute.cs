using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        public HttpPostAttribute(string route) : base(route, "POST") {}
    }
}
