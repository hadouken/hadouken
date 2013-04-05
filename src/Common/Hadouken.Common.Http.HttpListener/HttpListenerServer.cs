using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Common.Http.HttpListener
{
    public class HttpListenerServer : HttpServer
    {
        private System.Net.HttpListener _httpListener;
        private HttpListenerBasicIdentity _credential;

        public override void Start(Uri binding, NetworkCredential credential)
        {
            if (_httpListener != null)
                return;

            _credential = new HttpListenerBasicIdentity(credential.UserName, credential.Password);

            _httpListener = new System.Net.HttpListener();
            _httpListener.Prefixes.Add(binding.ToString());
            _httpListener.Start();

            _httpListener.BeginGetContext(BeginGetContext, null);
        }

        private void BeginGetContext(IAsyncResult ar)
        {
            var context = _httpListener.EndGetContext(ar);
            _httpListener.BeginGetContext(BeginGetContext, null);

            if (!IsAuthenticated(context.User.Identity as HttpListenerBasicIdentity))
                return;

            Task.Factory.StartNew(() => OnHttpRequest(new HttpContext(context)));
        }

        private bool IsAuthenticated(HttpListenerBasicIdentity identity)
        {
            if (identity == null)
                return false;

            return (identity == _credential);
        }

        public override void Stop()
        {
            _httpListener.Stop();
            _httpListener = null;
        }
    }
}
