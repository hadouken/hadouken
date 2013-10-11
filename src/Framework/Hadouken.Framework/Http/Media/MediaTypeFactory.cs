using System.Collections.Generic;

namespace Hadouken.Framework.Http.Media
{
    public class MediaTypeFactory : IMediaTypeFactory
    {
        private readonly object _mediaTypesLock = new object();
        private readonly IDictionary<string, IMediaType> _mediaTypes = new Dictionary<string, IMediaType>();

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

        public static IMediaTypeFactory CreateDefault()
        {
            var factory = new MediaTypeFactory();
            factory.Add(".html", new BasicMediaType("text/html"));
            factory.Add(".js", new BasicMediaType("text/javascript"));
            factory.Add(".css", new BasicMediaType("text/css"));
            factory.Add(".woff", new BasicMediaType("application/font-woff"));
            factory.Add(".ttf", new BasicMediaType("application/x-font-ttf"));
            factory.Add(".svg", new BasicMediaType("application/octet-stream"));

            return factory;
        }
    }
}