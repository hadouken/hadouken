using System;

namespace Hadouken.Common.Timers
{
    public class TimerFactory : ITimerFactory
    {
        public ITimer Create(int interval, Action callback)
        {
            return new ThreadedTimer(interval, callback);
        }
    }
}