using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public abstract class Controller : IController
    {
        public IHttpContext Context { get; set; }

        public ActionResult View(string name)
        {
            return new ViewResult(name);
        }
    }
}
