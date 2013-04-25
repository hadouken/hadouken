using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Common.Http.Mvc;

namespace Hadouken.Common.Http.HttpListener
{
    public class HttpListenerServer : IHttpFileSystemServer
    {
        private readonly System.Net.HttpListener _httpListener;
        private readonly HttpListenerBasicIdentity _credential;
        private readonly Uri _binding;
        private readonly string _basePath;

        public HttpListenerServer(Uri binding, NetworkCredential credential, string basePath)
        {
            _binding = binding;
            _basePath = basePath;

            _httpListener = new System.Net.HttpListener();
            _httpListener.Prefixes.Add(binding.ToString());
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            _credential = new HttpListenerBasicIdentity(credential.UserName, credential.Password);            
        }

        public void Start()
        {
            if (_httpListener == null) return;

            _httpListener.Start();
            _httpListener.BeginGetContext(BeginGetContext, null);
        }

        private void BeginGetContext(IAsyncResult ar)
        {
            try
            {
                var context = _httpListener.EndGetContext(ar);

                if (!IsAuthenticated(context.User.Identity as HttpListenerBasicIdentity))
                    return;

                Task.Factory.StartNew(() => OnHttpRequest(new HttpContext(context)));

                _httpListener.BeginGetContext(BeginGetContext, null);
            }
            catch (HttpListenerException)
            {
                //TODO: better catch clause
            }
        }

        private void OnHttpRequest(IHttpContext context)
        {
            // Check file system for file

            context.Response.OutputStream.Close();
            context.Response.Close();
        }

        private bool IsAuthenticated(HttpListenerBasicIdentity identity)
        {
            if (identity == null)
                return false;

            return (identity.Name == _credential.Name && identity.Password == _credential.Password);
        }

        private static string GetContentType(string extension)
        {
            switch (extension)
            {
                case ".js":
                    return "text/javascript";

                case ".css":
                    return "text/css";
            }

            return "text/html";
        }

        public void Stop()
        {
            _httpListener.Stop();
        }
    }
}
