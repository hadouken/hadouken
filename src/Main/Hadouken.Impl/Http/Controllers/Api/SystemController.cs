using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;

namespace Hadouken.Impl.Http.Controllers.Api
{
    public class SystemController : Controller
    {
        [HttpGet]
        [Route("/api/system")]
        public ActionResult Get()
        {
            var version = typeof(Kernel).Assembly.GetName().Version.ToString();

            return Json(new
            {
                version = version
            });
        }
    }
}
