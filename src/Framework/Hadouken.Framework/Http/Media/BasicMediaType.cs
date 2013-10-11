namespace Hadouken.Framework.Http.Media
{
    public class BasicMediaTypeHandler : IMediaTypeHandler
    {
        public BasicMediaTypeHandler(string mediaType)
        {
            MediaType = mediaType;
        }

        public string MediaType { get; private set; }

        public virtual IMedia Handle(IMedia media)
        {
            return media;
        }
    }
}
