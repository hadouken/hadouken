using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public class HttpPutAttribute : HttpMethodAttribute
    {
        public HttpPutAttribute(string route) : base(route, "PUT")
        {
        }
    }
}
