using Hadouken.Data;
using Hadouken.Http;
using HdknPlugins.Rss.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.Rss.Http.Api
{
    [ApiAction("rss-setfeeds")]
    public class SetFeeds : ApiAction
    {
        private readonly IDataRepository _dataRepository;

        public SetFeeds(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public override ActionResult Execute()
        {
            var feeds = BindModel<Feed[]>();

            foreach (var feed in feeds)
            {
                _dataRepository.SaveOrUpdate(feed);
            }

            return Json(feeds);
        }
    }
}
