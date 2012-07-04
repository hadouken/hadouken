using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using System.Net;

namespace Hadouken.Impl.Http
{
    public class HttpContext : IHttpContext
    {
        private HttpListenerContext _context;

        private HttpRequest _request;
        private HttpResponse _response;

        public HttpContext(HttpListenerContext listenerContext)
        {
            _context = listenerContext;
        }

        public IHttpRequest Request
        {
            get { if (_request == null) { _request = new HttpRequest(_context.Request); } return _request; }
        }

        public IHttpResponse Response
        {
            get { if (_response == null) { _response = new HttpResponse(_context.Response); } return _response; }
        }

        public System.Security.Principal.IPrincipal User
        {
            get { return _context.User; }
        }
    }
}
