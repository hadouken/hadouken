using System;
using System.Collections.Generic;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Rss.Data;
using Hadouken.Extensions.Rss.Data.Models;

namespace Hadouken.Extensions.Rss.Services {
    public sealed class RssService : IJsonRpcService {
        private readonly IRssRepository _rssRepository;

        public RssService(IRssRepository rssRepository) {
            if (rssRepository == null) {
                throw new ArgumentNullException("rssRepository");
            }
            this._rssRepository = rssRepository;
        }

        [JsonRpcMethod("rss.feeds.create")]
        public Feed CreateFeed(Feed feed) {
            this._rssRepository.CreateFeed(feed);
            return feed;
        }

        [JsonRpcMethod("rss.filters.create")]
        public Filter CreateFilter(Filter filter) {
            this._rssRepository.CreateFilter(filter);
            return filter;
        }

        [JsonRpcMethod("rss.modifiers.create")]
        public Modifier CreateModifier(Modifier modifier) {
            this._rssRepository.CreateModifier(modifier);
            return modifier;
        }

        [JsonRpcMethod("rss.feeds.delete")]
        public void DeleteFeed(int feedId) {
            this._rssRepository.DeleteFeed(feedId);
        }

        [JsonRpcMethod("rss.filters.delete")]
        public void DeleteFilter(int filterId) {
            this._rssRepository.DeleteFilter(filterId);
        }

        [JsonRpcMethod("rss.modifiers.delete")]
        public void DeleteModifier(int modifierId) {
            this._rssRepository.DeleteModifier(modifierId);
        }

        [JsonRpcMethod("rss.feeds.getAll")]
        public IEnumerable<Feed> GetFeeds() {
            return this._rssRepository.GetFeeds();
        }

        [JsonRpcMethod("rss.filters.getByFeedId")]
        public IEnumerable<Filter> GetFiltersByFeedId(int feedId) {
            return this._rssRepository.GetFiltersByFeedId(feedId);
        }

        [JsonRpcMethod("rss.modifiers.getByFilterId")]
        public IEnumerable<Modifier> GetModifiersByFilterId(int filterId) {
            return this._rssRepository.GetModifiersByFilterId(filterId);
        }

        [JsonRpcMethod("rss.feeds.update")]
        public void UpdateFeed(Feed feed) {
            this._rssRepository.UpdateFeed(feed);
        }

        [JsonRpcMethod("rss.filters.update")]
        public void UpdateFilter(Filter filter) {
            this._rssRepository.UpdateFilter(filter);
        }

        [JsonRpcMethod("rss.modifiers.update")]
        public void UpdateModifier(Modifier modifier) {
            this._rssRepository.UpdateModifier(modifier);
        }
    }
}