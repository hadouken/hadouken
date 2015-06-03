using System;

namespace Hadouken.Extensions.Pushalot.Http {
    public sealed class Message {
        private readonly string _authorizationToken;
        private readonly string _body;

        public Message(string authorizationToken, string body) {
            if (authorizationToken == null) {
                throw new ArgumentNullException("authorizationToken");
            }
            if (body == null) {
                throw new ArgumentNullException("body");
            }

            this._authorizationToken = authorizationToken;
            this._body = body;
        }

        public string AuthorizationToken {
            get { return this._authorizationToken; }
        }

        public string Body {
            get { return this._body; }
        }

        public string Title { get; set; }

        public string Source {
            get { return "Hadouken"; }
        }
    }
}