using System;
using System.Collections.Generic;
using Hadouken.Configuration;
using Hadouken.Http.Api.Models;

namespace Hadouken.Http.Api
{
    public class PluginRepository : IPluginRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IApiConnection _apiConnection;

        public PluginRepository(IConfiguration configuration, IApiConnection apiConnection)
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

        public IEnumerable<PluginListItem> Search(string query)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<PluginListItem> GetAll()
        {
            var uri = new Uri(_configuration.Plugins.RepositoryUri, "plugins");
            return _apiConnection.GetAsync<IEnumerable<PluginListItem>>(uri).Result;
        }

        public Plugin GetById(string id)
        {
            var uri = new Uri(_configuration.Plugins.RepositoryUri, "plugins/" + id);
            return _apiConnection.GetAsync<Plugin>(uri).Result;
        }
    }
}