using System;
using System.Threading;

namespace Hadouken.Common.Timers
{
    public class ThreadedTimer : ITimer
    {
        private readonly int _interval;
        private readonly Action _callback;
        private Timer _timer;
        private int _ticks;

        public ThreadedTimer(int interval, Action callback)
        {
            _interval = interval;
            _callback = callback;
        }

        public long Ticks
        {
            get { return _ticks; }
        }

        public void Start()
        {
            // Make the first call have ticks=0
            _ticks = -1;

            _timer = new Timer(_ =>
            {
                Interlocked.Increment(ref _ticks);
                _callback();
            }, null, 0, _interval);
        }

        public void Stop()
        {
            _timer.Dispose();
            _timer = null;
        }
    }
}