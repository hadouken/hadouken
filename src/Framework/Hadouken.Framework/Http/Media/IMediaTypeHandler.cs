namespace Hadouken.Framework.Http.Media
{
    public interface IMediaTypeHandler
    {
        string MediaType { get; }

        IMedia Handle(IMedia media);
    }
}
