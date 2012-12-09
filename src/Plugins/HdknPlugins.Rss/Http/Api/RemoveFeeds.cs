using Hadouken.Data;
using Hadouken.Http;
using HdknPlugins.Rss.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.Rss.Http.Api
{
    [ApiAction("rss-remfeeds")]
    public class RemoveFeeds : ApiAction
    {
        private readonly IDataRepository _dataRepository;

        public RemoveFeeds(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public override ActionResult Execute()
        {
            var feeds = BindModel<Feed[]>();

            foreach (var feed in feeds)
            {
                _dataRepository.Delete(feed);
            }

            return Json(feeds);
        }
    }
}
