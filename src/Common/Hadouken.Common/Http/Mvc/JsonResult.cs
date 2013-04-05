using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public class JsonResult : ActionResult
    {
        private readonly object _object;

        public JsonResult(object obj)
        {
            _object = obj;
        }

        public override void Execute(IHttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
