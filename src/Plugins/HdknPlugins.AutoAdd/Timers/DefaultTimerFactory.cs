using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.AutoAdd.Timers
{
    public class DefaultTimerFactory : ITimerFactory
    {
        public ITimer CreateTimer()
        {
            return new ThreadedTimer();
        }
    }
}
