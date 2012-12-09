using Hadouken;

namespace HdknPlugins.Rss.Timers
{
    public interface ITimerFactory : IComponent
    {
        ITimer CreateTimer();
    }
}
