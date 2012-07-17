using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hadouken.Impl.Http;
using Moq;
using Hadouken.Data;
using Hadouken.IO;
using System.Net;

namespace Hadouken.UnitTests.Http
{
    [TestFixture]
    public class HttpServerTests
    {

        [Test]
        public void Can_start_and_stop_HTTP_server()
        {
            var repo = new Mock<IDataRepository>();
            var fs = new Mock<IFileSystem>();

            var server = new DefaultHttpServer(repo.Object, fs.Object);
            server.Start();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(server.ListenUri);

            req.Method = "GET";
            
            req.Timeout = 5000;

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Assert.IsTrue(resp.StatusCode == HttpStatusCode.OK);

            server.Stop();
        }

        public void SetBasicAuthHeader(WebRequest request, String userName, String userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
        }

    }
}
