using System;
using System.Collections.Generic;
using Hadouken.Configuration;
using Hadouken.Http.Api.Models;

namespace Hadouken.Http.Api
{
    public class ReleasesRepository : IReleasesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IApiConnection _apiConnection;

        public ReleasesRepository(IConfiguration configuration, IApiConnection apiConnection)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (apiConnection == null)
            {
                throw new ArgumentNullException("apiConnection");
            }

            _configuration = configuration;
            _apiConnection = apiConnection;
        }

        public IEnumerable<ReleaseItem> GetAll()
        {
            var uri = new Uri(_configuration.Plugins.RepositoryUri, "releases");
            return _apiConnection.GetAsync<IEnumerable<ReleaseItem>>(uri).Result;
        }
    }
}