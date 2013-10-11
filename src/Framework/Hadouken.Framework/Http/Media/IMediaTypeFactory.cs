namespace Hadouken.Framework.Http.Media
{
    public interface IMediaTypeFactory
    {
        IMediaType Get(string extension);

        void Add(string extension, IMediaType mediaType);

        void Replace(string extension, IMediaType replacementMediaType);
    }
}
