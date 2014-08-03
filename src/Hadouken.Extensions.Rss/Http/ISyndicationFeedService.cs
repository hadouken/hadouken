using System.ServiceModel.Syndication;

namespace Hadouken.Extensions.Rss.Http
{
    public interface ISyndicationFeedService
    {
        SyndicationFeed GetFeed(string url);
    }
}
