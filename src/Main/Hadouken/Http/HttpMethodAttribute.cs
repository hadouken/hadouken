using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class HttpMethodAttribute : Attribute
    {
        public HttpMethodAttribute(string method)
        {
            Method = method;
        }

        public string Method { get; private set; }
    }
}
