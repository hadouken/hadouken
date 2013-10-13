namespace Hadouken.Framework.Http.Media
{
    public interface IMediaTypeHandler
    {
        string MediaType { get; }

        HandleResult Handle(IMedia media);
    }
}
