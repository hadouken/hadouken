using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        public HttpPostAttribute() : base("POST") { }
    }
}
