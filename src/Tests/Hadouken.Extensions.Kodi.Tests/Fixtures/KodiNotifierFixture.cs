using Hadouken.Common.Data;
using Hadouken.Extensions.Kodi.Http;
using NSubstitute;

namespace Hadouken.Extensions.Kodi.Tests.Fixtures {
    internal sealed class KodiNotifierFixture {
        public KodiNotifierFixture() {
            this.KodiClient = Substitute.For<IKodiClient>();
            this.KeyValueStore = Substitute.For<IKeyValueStore>();
        }

        public IKodiClient KodiClient { get; set; }
        public IKeyValueStore KeyValueStore { get; set; }

        public KodiNotifier CreateNotifier() {
            return new KodiNotifier(this.KodiClient, this.KeyValueStore);
        }
    }
}