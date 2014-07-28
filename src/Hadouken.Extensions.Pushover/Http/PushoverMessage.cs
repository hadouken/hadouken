using System;

namespace Hadouken.Extensions.Pushover.Http
{
    public sealed class PushoverMessage
    {
        private readonly string _token;
        private readonly string _user;
        private readonly string _message;

        public PushoverMessage(string token, string user, string message)
        {
            if (token == null) throw new ArgumentNullException("token");
            if (user == null) throw new ArgumentNullException("user");
            if (message == null) throw new ArgumentNullException("message");

            _token = token;
            _user = user;
            _message = message;
        }

        public string Token
        {
            get { return _token; }
        }

        public string User
        {
            get { return _user; }
        }

        public string Message
        {
            get { return _message; }
        }

        public string Title { get; set; }
    }
}
