using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Hadouken.Http
{
    public class JsonResult : ActionResult
    {
        public object Data { get; set; }

        public override void Execute(IHttpContext context)
        {
            context.Response.ContentType = "application/json";

            byte[] data = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(Data));

            context.Response.OutputStream.Write(data, 0, data.Length);
        }
    }
}
