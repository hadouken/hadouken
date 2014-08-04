using System.Collections.Generic;
using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss.Data
{
    public interface IRssRepository
    {
        void CreateFeed(Feed feed);

        void CreateFilter(Filter filter);

        void CreateModifier(Modifier modifier);

        void DeleteFeed(int feedId);

        void DeleteFilter(int filterId);

        void DeleteModifier(int modifierId);

        IEnumerable<Feed> GetFeeds();

        IEnumerable<Filter> GetFiltersByFeedId(int feedId);

        IEnumerable<Modifier> GetModifiersByFilterId(int filterId);

        void UpdateFeed(Feed feed);

        void UpdateFilter(Filter filter);

        void UpdateModifier(Modifier modifier);
    }
}
