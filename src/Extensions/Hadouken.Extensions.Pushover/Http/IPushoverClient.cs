namespace Hadouken.Extensions.Pushover.Http
{
    public interface IPushoverClient
    {
        void Send(PushoverMessage message);
    }
}
