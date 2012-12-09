using Hadouken.Data;
using Hadouken.Http;
using HdknPlugins.Rss.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.Rss.Http.Api
{
    [ApiAction("rss-getfeeds")]
    public class GetFeeds : ApiAction
    {
        private readonly IDataRepository _dataRepository;

        public GetFeeds(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public override ActionResult Execute()
        {
            return Json(_dataRepository.List<Feed>());
        }
    }
}
