using Hadouken.Framework.IO;

namespace Hadouken.Framework.Http.Media
{
    public class BasicMediaTypeHandler : IMediaTypeHandler
    {
        private readonly IFileSystem _fileSystem;

        public BasicMediaTypeHandler(IFileSystem fileSystem, string mediaType)
        {
            _fileSystem = fileSystem;
            MediaType = mediaType;
        }

        public string MediaType { get; protected set; }

        public virtual HandleResult Handle(IMedia media)
        {
            if (_fileSystem.FileExists(media.Path))
            {
                return new ContentResult(_fileSystem, MediaType, media.Path);
            }

            return new HttpNotFoundResult();
        }
    }
}
