using System;

namespace Hadouken.Extensions.Pushalot.Http
{
    public sealed class Message
    {
        private readonly string _authorizationToken;
        private readonly string _body;

        public Message(string authorizationToken, string body)
        {
            if (authorizationToken == null) throw new ArgumentNullException("authorizationToken");
            if (body == null) throw new ArgumentNullException("body");

            _authorizationToken = authorizationToken;
            _body = body;
        }

        public string AuthorizationToken { get { return _authorizationToken; } }

        public string Body { get { return _body; } }

        public string Title { get; set; }

        public string Source { get { return "Hadouken"; } }
    }
}
