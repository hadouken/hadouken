namespace Hadouken.Extensions.Pushbullet.Http
{
    public interface IPushbulletClient
    {
        void Send(string accessToken, Note note);
    }
}
