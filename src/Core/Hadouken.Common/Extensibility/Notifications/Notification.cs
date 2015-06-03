using System;

namespace Hadouken.Common.Extensibility.Notifications {
    public class Notification {
        private readonly string _message;
        private readonly string _title;
        private readonly NotificationType _type;

        public Notification(NotificationType type, string title, string message) {
            if (title == null) {
                throw new ArgumentNullException("title");
            }
            if (message == null) {
                throw new ArgumentNullException("message");
            }
            this._type = type;
            this._title = title;
            this._message = message;
        }

        public NotificationType Type {
            get { return this._type; }
        }

        public string Title {
            get { return this._title; }
        }

        public string Message {
            get { return this._message; }
        }
    }
}