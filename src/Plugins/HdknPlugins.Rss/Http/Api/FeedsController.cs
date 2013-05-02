using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Hadouken.Common.Data;
using HdknPlugins.Rss.Data.Models;

namespace HdknPlugins.Rss.Http.Api
{
    public class FeedsController : ApiController
    {
        private readonly IDataRepository _repository;

        public FeedsController(IDataRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            _repository = repository;
        }

        public IEnumerable<Feed> Get()
        {
            return _repository.List<Feed>();
        } 
    }
}
