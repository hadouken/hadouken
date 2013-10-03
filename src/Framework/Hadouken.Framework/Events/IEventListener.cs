using System;

namespace Hadouken.Framework.Events
{
    public interface IEventListener
    {
        void Subscribe<T>(string eventName, Action<T> callback);
    }
}
