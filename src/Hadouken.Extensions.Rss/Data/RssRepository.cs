using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss.Data
{
    [Component]
    public class RssRepository : IRssRepository
    {
        private readonly IDbConnection _connection;

        public RssRepository(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            _connection = connection;
        }

        public void CreateFeed(Feed feed)
        {
            var query = @"insert into Rss_Feed (Name, Url, PollInterval, LastUpdatedTime) values(@Name, @Url, @PollInterval, @LastUpdatedTime); select last_insert_rowid();";
            feed.Id = _connection.Query<int>(query, feed).First();
        }

        public void CreateFilter(Filter filter)
        {
            var query = @"insert into Rss_Filter (FeedId, IncludePattern, ExcludePattern, AutoStart) values(@FeedId, @IncludePattern, @ExcludePattern, @AutoStart); select last_insert_rowid();";
            filter.Id = _connection.Query<int>(query, filter).First();
        }

        public void CreateModifier(Modifier modifier)
        {
            var query = @"insert into Rss_Modifier (FilterId, Target, Value) values(@FilterId, @Target, @Value); select last_insert_rowid();";
            modifier.Id = _connection.Query<int>(query, modifier).First();
        }

        public void DeleteFeed(int feedId)
        {
            var query = @"delete from Rss_Feed where Id = @Id";
            _connection.Execute(query, new {Id = feedId});
        }

        public void DeleteFilter(int filterId)
        {
            var query = @"delete from Rss_Filter where Id = @Id";
            _connection.Execute(query, new {Id = filterId});
        }

        public void DeleteModifier(int modifierId)
        {
            var query = @"delete from Rss_Modifier where Id = @Id";
            _connection.Execute(query, new {Id = modifierId});
        }

        public IEnumerable<Feed> GetFeeds()
        {
            var query = @"select f.Id, f.Name, f.Url, f.PollInterval, f.LastUpdatedTime from Rss_Feed f";
            return _connection.Query<Feed>(query);
        }

        public IEnumerable<Filter> GetFiltersByFeedId(int feedId)
        {
            var query = @"select f.Id, f.FeedId, f.IncludePattern, f.ExcludePattern from Rss_Filter f where f.FeedId = @FeedId";
            return _connection.Query<Filter>(query, new {FeedId = feedId});
        }

        public IEnumerable<Modifier> GetModifiersByFilterId(int filterId)
        {
            var query = @"select m.Id, m.FilterId, m.Target, m.Value from Rss_Modifier m where m.FilterId = @FilterId";
            return _connection.Query<Modifier>(query, new {FilterId = filterId});
        }

        public void UpdateFeed(Feed feed)
        {
            var query = @"update Rss_Feed set Name = @Name, Url = @Url, PollInterval = @PollInterval where Id = @Id";
            _connection.Execute(query, feed);
        }

        public void UpdateFilter(Filter filter)
        {
            var query = @"update Rss_Filter set IncludePattern = @IncludePattern, ExcludePattern = @ExcludePattern, AutoStart = @AutoStart where Id = @Id";
            _connection.Execute(query, filter);
        }

        public void UpdateModifier(Modifier modifier)
        {
            var query = @"update Rss_Modifier set [Target] = @Target, Value = @Value where Id = @Id";
            _connection.Execute(query, modifier);
        }
    }
}