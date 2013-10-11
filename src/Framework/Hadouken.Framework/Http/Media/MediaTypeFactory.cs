using System.Collections.Generic;

namespace Hadouken.Framework.Http.Media
{
    public class MediaTypeFactory : IMediaTypeFactory
    {
        private readonly object _mediaTypesLock = new object();
        private readonly IDictionary<string, IMediaType> _mediaTypes;

        public MediaTypeFactory()
        {
            _mediaTypes = CreateDefault();
        }

        public IMediaType Get(string extension)
        {
            lock (_mediaTypesLock)
            {
                if (_mediaTypes.ContainsKey(extension))
                    return _mediaTypes[extension];
            }

            return null;
        }

        public void Add(string extension, IMediaType mediaType)
        {
            lock (_mediaTypesLock)
            {
                _mediaTypes.Add(extension, mediaType);
            }
        }

        public void Replace(string extension, IMediaType mediaType)
        {
            lock (_mediaTypesLock)
            {
                if (_mediaTypes.ContainsKey(extension))
                    _mediaTypes.Remove(extension);

                _mediaTypes.Add(extension, mediaType);
            }
        }

        private IDictionary<string, IMediaType> CreateDefault()
        {
            return new Dictionary<string, IMediaType>
            {
                {".html", new BasicMediaType("text/html")},
                {".js", new BasicMediaType("text/javascript")},
                {".css", new BasicMediaType("text/css")},
                {".woff", new BasicMediaType("application/font-woff")},
                {".ttf", new BasicMediaType("application/x-font-ttf")},
                {".svg", new BasicMediaType("application/octet-stream")}
            };
        }
    }
}