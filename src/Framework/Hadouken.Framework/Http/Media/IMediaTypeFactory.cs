namespace Hadouken.Framework.Http.Media
{
    public interface IMediaTypeFactory
    {
        IMediaTypeHandler Get(string extension);

        void Add(string extension, IMediaTypeHandler mediaTypeHandler);

        void Replace(string extension, IMediaTypeHandler replacementMediaTypeHandler);
    }
}
