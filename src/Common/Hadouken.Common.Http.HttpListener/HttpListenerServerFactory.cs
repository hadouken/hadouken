using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Http.Mvc;

namespace Hadouken.Common.Http.HttpListener
{
    [Component]
    public class HttpListenerServerFactory : IHttpServerFactory
    {
        private readonly IController[] _controllers;

        public HttpListenerServerFactory(IController[] controllers)
        {
            _controllers = controllers;
        }

        public IHttpServer Create(string binding, System.Net.NetworkCredential credential)
        {
            return new HttpListenerServer(new Uri(binding), credential, _controllers);
        }
    }
}
