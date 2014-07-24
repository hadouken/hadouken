using System;

namespace Hadouken.Common.Extensibility.Notifications
{
    public class Notification
    {
        private readonly string _title;
        private readonly string _message;

        public Notification(string title, string message)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (message == null) throw new ArgumentNullException("message");
            _title = title;
            _message = message;
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
