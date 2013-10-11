namespace Hadouken.Framework.Http.Media
{
    public interface IMediaType
    {
        string ContentType { get; }

        byte[] Transform(byte[] data);
    }
}
