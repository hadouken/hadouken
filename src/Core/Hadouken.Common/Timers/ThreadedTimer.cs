using System;
using System.Threading;

namespace Hadouken.Common.Timers {
    public class ThreadedTimer : ITimer {
        private readonly Action _callback;
        private readonly int _interval;
        private int _ticks;
        private Timer _timer;

        public ThreadedTimer(int interval, Action callback) {
            this._interval = interval;
            this._callback = callback;
        }

        public long Ticks {
            get { return this._ticks; }
        }

        public void Start() {
            // Make the first call have ticks=0
            this._ticks = -1;

            this._timer = new Timer(_ => {
                Interlocked.Increment(ref this._ticks);
                this._callback();
            }, null, 0, this._interval);
        }

        public void Stop() {
            this._timer.Dispose();
            this._timer = null;
        }
    }
}