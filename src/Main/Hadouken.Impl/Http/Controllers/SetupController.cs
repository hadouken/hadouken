using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Http;

namespace Hadouken.Impl.Http.Handlers
{
    public class SetupController : Controller
    {
        [HttpGet]
        [Route("/setup")]
        public ActionResult Index()
        {
            return View("setup.html");
        }
    }
}
