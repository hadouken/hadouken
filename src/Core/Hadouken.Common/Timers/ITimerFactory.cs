using System;

namespace Hadouken.Common.Timers {
    public interface ITimerFactory {
        ITimer Create(int interval, Action callback);
    }
}