namespace Hadouken.Framework.Http.Media
{
    public class BasicMediaTypeHandler : IMediaTypeHandler
    {
        public BasicMediaTypeHandler(string mediaType)
        {
            MediaType = mediaType;
        }

        public string MediaType { get; protected set; }

        public virtual IMedia Handle(IMedia media)
        {
            return media;
        }
    }
}
