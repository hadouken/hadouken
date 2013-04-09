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
        private readonly System.Net.HttpListener _httpListener;
        private readonly HttpListenerBasicIdentity _credential;

        public HttpListenerServer(Uri binding, NetworkCredential credential) : base(binding, credential)
        {
            _httpListener = new System.Net.HttpListener();
            _httpListener.Prefixes.Add(binding.ToString());
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            _credential = new HttpListenerBasicIdentity(credential.UserName, credential.Password);            
        }

        public override void Start()
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

        private bool IsAuthenticated(HttpListenerBasicIdentity identity)
        {
            if (identity == null)
                return false;

            return (identity.Name == _credential.Name && identity.Password == _credential.Password);
        }

        public override void Stop()
        {
            _httpListener.Stop();
        }
    }
}
