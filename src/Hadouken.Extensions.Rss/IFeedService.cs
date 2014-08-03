using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss
{
    public interface IFeedService
    {
        void Check(Feed feed);
    }
}
