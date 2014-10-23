using System;

namespace Hadouken.Common.Extensibility.Notifications
{
    public class Notification
    {
        private readonly NotificationType _type;
        private readonly string _title;
        private readonly string _message;

        public Notification(NotificationType type, string title, string message)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (message == null) throw new ArgumentNullException("message");
            _type = type;
            _title = title;
            _message = message;
        }

        public NotificationType Type
        {
            get { return _type; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string Message
        {
            get { return _message; }
        }
    }
}
