namespace Hadouken.Common.Timers
{
    public interface ITimer
    {
        long Ticks { get; }

        void Start();

        void Stop();
    }
}
