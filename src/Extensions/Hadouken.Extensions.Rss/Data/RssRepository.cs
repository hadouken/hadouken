using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss.Data {
    [Component]
    public class RssRepository : IRssRepository {
        private readonly IDbConnection _connection;

        public RssRepository(IDbConnection connection) {
            if (connection == null) {
                throw new ArgumentNullException("connection");
            }
            this._connection = connection;
        }

        public void CreateFeed(Feed feed) {
            const string query = @"insert into Rss_Feed (Name, Url, PollInterval, LastUpdatedTime) values(@Name, @Url, @PollInterval, @LastUpdatedTime); select last_insert_rowid();";
            feed.Id = this._connection.Query<int>(query, feed).First();
        }

        public void CreateFilter(Filter filter) {
            const string query = @"insert into Rss_Filter (FeedId, IncludePattern, ExcludePattern, AutoStart) values(@FeedId, @IncludePattern, @ExcludePattern, @AutoStart); select last_insert_rowid();";
            filter.Id = this._connection.Query<int>(query, filter).First();
        }

        public void CreateModifier(Modifier modifier) {
            const string query = @"insert into Rss_Modifier (FilterId, Target, Value) values(@FilterId, @Target, @Value); select last_insert_rowid();";
            modifier.Id = this._connection.Query<int>(query, modifier).First();
        }

        public void DeleteFeed(int feedId) {
            const string query = @"delete from Rss_Feed where Id = @Id";
            this._connection.Execute(query, new {Id = feedId});
        }

        public void DeleteFilter(int filterId) {
            const string query = @"delete from Rss_Filter where Id = @Id";
            this._connection.Execute(query, new {Id = filterId});
        }

        public void DeleteModifier(int modifierId) {
            const string query = @"delete from Rss_Modifier where Id = @Id";
            this._connection.Execute(query, new {Id = modifierId});
        }

        public IEnumerable<Feed> GetFeeds() {
            const string query = @"select f.Id, f.Name, f.Url, f.PollInterval, f.LastUpdatedTime from Rss_Feed f";
            return this._connection.Query<Feed>(query);
        }

        public IEnumerable<Filter> GetFiltersByFeedId(int feedId) {
            const string query = @"select f.Id, f.FeedId, f.IncludePattern, f.ExcludePattern from Rss_Filter f where f.FeedId = @FeedId";
            return this._connection.Query<Filter>(query, new {FeedId = feedId});
        }

        public IEnumerable<Modifier> GetModifiersByFilterId(int filterId) {
            const string query = @"select m.Id, m.FilterId, m.Target, m.Value from Rss_Modifier m where m.FilterId = @FilterId";
            return this._connection.Query<Modifier>(query, new {FilterId = filterId});
        }

        public void UpdateFeed(Feed feed) {
            const string query = @"update Rss_Feed set Name = @Name, Url = @Url, PollInterval = @PollInterval where Id = @Id";
            this._connection.Execute(query, feed);
        }

        public void UpdateFilter(Filter filter) {
            const string query = @"update Rss_Filter set IncludePattern = @IncludePattern, ExcludePattern = @ExcludePattern, AutoStart = @AutoStart where Id = @Id";
            this._connection.Execute(query, filter);
        }

        public void UpdateModifier(Modifier modifier) {
            const string query = @"update Rss_Modifier set [Target] = @Target, Value = @Value where Id = @Id";
            this._connection.Execute(query, modifier);
        }

        public void UpdateFeedLastUpdatedTime(int feedId, DateTime lastUpdatedTime) {
            const string query = @"update Rss_Feed set LastUpdatedTime = @LastUpdatedTime where Id = @Id";
            this._connection.Execute(query, new {Id = feedId, LastUpdatedTime = lastUpdatedTime});
        }
    }
}