using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("getsysteminfo")]
    public class GetSystemInfo : ApiAction
    {
        public override ActionResult Execute()
        {
            return Json(new
                            {
                                version = typeof (Kernel).Assembly.GetName().Version
                            });
        }
    }
}
