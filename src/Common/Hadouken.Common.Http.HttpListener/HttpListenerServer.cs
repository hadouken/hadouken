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
    public class HttpListenerServer : IHttpServer
    {
        private readonly System.Net.HttpListener _httpListener;
        private readonly HttpListenerBasicIdentity _credential;
        private readonly Uri _binding;
        private readonly IController[] _controllers;

        public HttpListenerServer(Uri binding, NetworkCredential credential, IController[] controllers)
        {
            _controllers = controllers;

            _binding = binding;

            _httpListener = new System.Net.HttpListener();
            _httpListener.Prefixes.Add(binding.ToString());
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            _credential = new HttpListenerBasicIdentity(credential.UserName, credential.Password);            
        }

        public FileLocationType FileLocationType { get; set; }

        public string FileLocationBase { get; set; }

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
            ActionResult result;

            if (GetFile(context, out result))
            {
                result.Execute(context);
            }

            // Check the file location for the specified type.

            // If file is found, return the result we got

            // Else, check if we have a matching controller

            // If we do, return that result

            // Else, return HttpNotFound

            context.Response.OutputStream.Close();
            context.Response.Close();
        }

        private bool GetFile(IHttpContext context, out ActionResult result)
        {
            result = new HttpNotFoundResult();
            byte[] data = null;

            switch (FileLocationType)
            {
                case FileLocationType.EmbeddedResource:
                    data = GetFileFromEmbeddedResource(context);
                    break;
                
                case FileLocationType.FileSystem:
                    //data = GetFileFromFileSystem();
                    break;
            }

            if (data != null)
            {
                result = new ContentResult(data, GetContentType(Path.GetExtension(context.Request.Url.PathAndQuery)));
                return true;
            }

            return false;
        }

        private byte[] GetFileFromEmbeddedResource(IHttpContext context)
        {
            // Base = HdknPlugins.AutoAdd.UI
            // Path = /plugins/autoadd/boot.js
            // Result = Hadouken.Plugins.AutoAdd.UI.boot.js

            // Base = HdknPlugins.AutoAdd.UI
            // Path = /plugins/autoadd/js/foo.js
            // Result = HdknPlugins.AutoAdd.UI.js.foo.js

            var bindingPath = _binding.PathAndQuery;
            var requestPath = context.Request.Url.PathAndQuery;

            var path = requestPath.Substring(bindingPath.Length);
            path = path.Replace("/", ".");
            path = String.Concat(FileLocationBase, ".", path);

            var asm = (from a in AppDomain.CurrentDomain.GetAssemblies()
                       from r in a.GetManifestResourceNames()
                       where r == path
                       select a).FirstOrDefault();

            if (asm == null)
                return null;

            using (var ms = new MemoryStream())
            using (var stream = asm.GetManifestResourceStream(path))
            {
                if (stream == null)
                    return null;

                stream.CopyTo(ms);

                return ms.ToArray();
            }
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
