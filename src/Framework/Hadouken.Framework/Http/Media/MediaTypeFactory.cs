using System.Collections.Generic;
using Hadouken.Framework.IO;

namespace Hadouken.Framework.Http.Media
{
    public class MediaTypeFactory : IMediaTypeFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly object _mediaTypesLock = new object();
        private readonly IDictionary<string, IMediaTypeHandler> _mediaTypes;

        public MediaTypeFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _mediaTypes = CreateDefault();
        }

        public IMediaTypeHandler Get(string extension)
        {
            lock (_mediaTypesLock)
            {
                if (_mediaTypes.ContainsKey(extension))
                    return _mediaTypes[extension];
            }

            return null;
        }

        public void Add(string extension, IMediaTypeHandler mediaTypeHandler)
        {
            lock (_mediaTypesLock)
            {
                _mediaTypes.Add(extension, mediaTypeHandler);
            }
        }

        public void Replace(string extension, IMediaTypeHandler mediaTypeHandler)
        {
            lock (_mediaTypesLock)
            {
                if (_mediaTypes.ContainsKey(extension))
                    _mediaTypes.Remove(extension);

                _mediaTypes.Add(extension, mediaTypeHandler);
            }
        }

        private IDictionary<string, IMediaTypeHandler> CreateDefault()
        {
            return new Dictionary<string, IMediaTypeHandler>
            {
                {".html", new BasicMediaTypeHandler(_fileSystem, "text/html")},
                {".js", new BasicMediaTypeHandler(_fileSystem, "text/javascript")},
                {".css", new BasicMediaTypeHandler(_fileSystem, "text/css")},
                {".woff", new BasicMediaTypeHandler(_fileSystem, "application/font-woff")},
                {".ttf", new BasicMediaTypeHandler(_fileSystem, "application/x-font-ttf")},
                {".svg", new BasicMediaTypeHandler(_fileSystem, "application/octet-stream")}
            };
        }
    }
}