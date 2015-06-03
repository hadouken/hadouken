using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss {
    public interface IFeedChecker {
        void Check(Feed feed);
    }
}