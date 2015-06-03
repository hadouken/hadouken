using System;

namespace Hadouken.Extensions.Pushbullet.Http {
    public sealed class Note {
        private readonly string _body;
        private readonly string _title;

        public Note(string title, string body) {
            if (title == null) {
                throw new ArgumentNullException("title");
            }
            if (body == null) {
                throw new ArgumentNullException("body");
            }
            this._title = title;
            this._body = body;
        }

        public string Title {
            get { return this._title; }
        }

        public string Body {
            get { return this._body; }
        }
    }
}