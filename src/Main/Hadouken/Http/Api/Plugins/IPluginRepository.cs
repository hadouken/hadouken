using System.Collections.Generic;
using Hadouken.Http.Api.Plugins.Models;

namespace Hadouken.Http.Api.Plugins
{
    public interface IPluginRepository
    {
        IEnumerable<PluginListItem> Search(string query);

        IEnumerable<PluginListItem> GetAll();

        Plugin GetById(string id);
    }
}
