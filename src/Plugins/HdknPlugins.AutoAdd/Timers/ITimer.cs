using Hadouken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.AutoAdd.Timers
{
    public interface ITimer
    {
        void SetCallback(int interval, Action callback);

        void Start();
        void Stop();
    }
}
