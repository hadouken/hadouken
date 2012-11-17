using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HdknPlugins.AutoAdd.Timers
{
    public class ThreadedTimer : ITimer
    {
        private int _interval;
        private Action _callback;
        private bool _isRunning;
        private Timer _timer;

        public void SetCallback(int interval, Action callback)
        {
            if(_isRunning)
                throw new InvalidOperationException("Can not change callback while running");

            _interval = interval;
            _callback = callback;
        }

        public void Start()
        {
            if(_isRunning)
                return;

            _isRunning = true;
            _timer = new Timer(_ => Tick(), null, 0, _interval);
        }

        private void Tick()
        {
            _callback();
        }

        public void Stop()
        {
            if(!_isRunning)
                return;

            _isRunning = false;

            _timer.Dispose();
            _timer = null;
        }
    }
}
