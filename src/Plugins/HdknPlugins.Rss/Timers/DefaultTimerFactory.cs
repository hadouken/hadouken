using Hadouken;
namespace HdknPlugins.Rss.Timers
{
    [Component]
    public class DefaultTimerFactory : ITimerFactory
    {
        public ITimer CreateTimer()
        {
            return new ThreadedTimer();
        }
    }
}
