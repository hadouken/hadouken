using System;

namespace Hadouken.Extensions.Pushbullet.Http
{
    public sealed class Note
    {
        private readonly string _title;
        private readonly string _body;

        public Note(string title, string body)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (body == null) throw new ArgumentNullException("body");
            _title = title;
            _body = body;
        }

        public string Title
        {
            get { return _title; }
        }

        public string Body
        {
            get { return _body; }
        }
    }
}
