using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Hadouken.Http;

namespace Hadouken
{
    public static class HttpListenerResponseExtensions
    {
        public static void Error(this IHttpResponse response, Exception exception)
        {
            string html = "<html><head><title>Internal Server Error</title></head><body>";

            html += "<h1>Internal Server Error</h1>";
            html += "<p>An error has occured.</p>";

            html += "<h2>Stack Trace</h2>";
            html += "<p><pre>" + exception.StackTrace.Replace(Environment.NewLine, "<br />") + "</pre></p>";

            html += "</body></html>";

            byte[] data = Encoding.UTF8.GetBytes(html);

            response.StatusCode = 500;
            response.OutputStream.Write(data, 0, data.Length);
        }

        public static void NotFound(this IHttpResponse response)
        {
            string html = "<html><head><title>Not found</title></head><body>";

            html += "<h1>Path not found</h1>";
            html += "<p>The path could not be found.</p>";

            html += "</body></html>";

            byte[] data = Encoding.UTF8.GetBytes(html);

            response.StatusCode = 500;
            response.OutputStream.Write(data, 0, data.Length);
        }

        public static void Unauthorized(this IHttpResponse response)
        {
            response.ContentType = "text/html";
            response.StatusCode = 401;

            byte[] unauthorized = Encoding.UTF8.GetBytes("<h1>401 - Unauthorized</h1>");

            response.OutputStream.Write(unauthorized, 0, unauthorized.Length);
        }
    }
}
