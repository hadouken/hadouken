using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Hadouken.Http;
using Moq;

namespace Hadouken.UnitTests.Http
{
    public static class HttpContextHelper
    {
        public static Mock<IHttpContext> CreateMockWithPostedFiles(params Stream[] fileContentStreams)
        {
            var list = new List<IHttpPostedFile>();

            foreach (var stream in fileContentStreams)
            {
                var file = new Mock<IHttpPostedFile>();
                file.SetupGet(f => f.InputStream).Returns(stream);

                list.Add(file.Object);
            }

            var rq = new Mock<IHttpRequest>();
            rq.SetupGet(r => r.Files).Returns(list);

            var rs = new Mock<IHttpResponse>();

            var ctx = new Mock<IHttpContext>();
            ctx.Setup(c => c.Request).Returns(rq.Object);
            ctx.Setup(c => c.Response).Returns(rs.Object);

            return ctx;
        }

        public static Mock<IHttpContext> CreateMockWithPostData(object obj)
        {
            var serializer = new JavaScriptSerializer();
            var data = serializer.Serialize(obj);

            var rq = new Mock<IHttpRequest>();
            rq.SetupGet(r => r.InputStream).Returns(new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var ctx = new Mock<IHttpContext>();
            ctx.SetupGet(c => c.Request).Returns(rq.Object);

            return ctx;
        }
    }
}
