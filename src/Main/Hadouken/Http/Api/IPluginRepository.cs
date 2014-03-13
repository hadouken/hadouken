using System.Collections.Generic;
using Hadouken.Http.Api.Models;

namespace Hadouken.Http.Api
{
    public interface IPluginRepository
    {
        IEnumerable<PluginListItem> Search(string query);

        IEnumerable<PluginListItem> GetAll();

        Plugin GetById(string id);
    }
}
