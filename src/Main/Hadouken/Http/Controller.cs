using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JsonFx;
using System.IO;

namespace Hadouken.Http
{
    public abstract class Controller : IController
    {
        public IHttpContext Context { get; set; }

        public ActionResult View(string name)
        {
            return new ViewResult(name);
        }

        public ActionResult Json(object data)
        {
            return new JsonResult() { Data = data };
        }

        public T BindModel<T>()
        {
            using (var reader = new StreamReader(Context.Request.InputStream))
            {
                string data = reader.ReadToEnd();
                return new JsonFx.Json.JsonReader().Read<T>(data);
            }
        }
    }
}
