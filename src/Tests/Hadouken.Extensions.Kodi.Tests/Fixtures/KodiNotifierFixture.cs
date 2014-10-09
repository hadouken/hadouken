using Hadouken.Common.Data;
using Hadouken.Extensions.Kodi.Http;
using NSubstitute;

namespace Hadouken.Extensions.Kodi.Tests.Fixtures
{
    internal sealed class KodiNotifierFixture
    {
        public KodiNotifierFixture()
        {
            KodiClient = Substitute.For<IKodiClient>();
            KeyValueStore = Substitute.For<IKeyValueStore>();
        }

        public IKodiClient KodiClient { get; set; }

        public IKeyValueStore KeyValueStore { get; set; }

        public KodiNotifier CreateNotifier()
        {
            return new KodiNotifier(KodiClient, KeyValueStore);
        }
    }
}
