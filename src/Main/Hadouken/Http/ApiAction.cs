using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Web.Script.Serialization;

namespace Hadouken.Http
{
    public abstract class ApiAction : IApiAction
    {
        public ActionResult View(string name)
        {
            return new ViewResult(name);
        }

        public ActionResult Json(object data)
        {
            return new JsonResult() { Data = data };
        }

        public ActionResult Redirect(string url)
        {
            return new RedirectResult(url);
        }

        public ActionResult FileNotFound()
        {
            return null;
        }

        public T BindModel<T>()
        {
            using (var reader = new StreamReader(Context.Request.InputStream))
            {
                string data = reader.ReadToEnd();

                var ser = new JavaScriptSerializer();
                return ser.Deserialize<T>(data);
            }
        }

        public virtual IHttpContext Context { get; set; }

        public abstract ActionResult Execute();
    }
}
