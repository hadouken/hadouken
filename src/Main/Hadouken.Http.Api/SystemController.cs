using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Hadouken.Common;

namespace Hadouken.Http.Api
{
    public class SystemController : ApiController
    {
        public object Get()
        {
            return new
                {
                    version = typeof (Kernel).Assembly.GetName().Version
                };
        }
    }
}
