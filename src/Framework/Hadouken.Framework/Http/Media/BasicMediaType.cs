namespace Hadouken.Framework.Http.Media
{
    public class BasicMediaType : IMediaType
    {
        public BasicMediaType(string contentType)
        {
            ContentType = contentType;
        }

        public string ContentType { get; private set; }

        public byte[] Transform(byte[] data)
        {
            return data;
        }
    }
}
