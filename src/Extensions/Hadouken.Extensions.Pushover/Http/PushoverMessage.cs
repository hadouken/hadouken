using System;

namespace Hadouken.Extensions.Pushover.Http {
    public sealed class PushoverMessage {
        private readonly string _message;
        private readonly string _token;
        private readonly string _user;

        public PushoverMessage(string token, string user, string message) {
            if (token == null) {
                throw new ArgumentNullException("token");
            }
            if (user == null) {
                throw new ArgumentNullException("user");
            }
            if (message == null) {
                throw new ArgumentNullException("message");
            }

            this._token = token;
            this._user = user;
            this._message = message;
        }

        public string Token {
            get { return this._token; }
        }

        public string User {
            get { return this._user; }
        }

        public string Message {
            get { return this._message; }
        }

        public string Title { get; set; }
    }
}