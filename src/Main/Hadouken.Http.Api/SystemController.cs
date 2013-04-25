using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Hadouken.Http.Api
{
    public class SystemController : ApiController
    {
        public object Get()
        {
            return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                    select asm.FullName);
        }
    }
}
