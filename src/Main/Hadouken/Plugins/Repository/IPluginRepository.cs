using System.Collections.Generic;
using Hadouken.Plugins.Repository.Models;

namespace Hadouken.Plugins.Repository
{
    public interface IPluginRepository
    {
        IEnumerable<PluginListItem> Search(string query);

        IEnumerable<PluginListItem> GetAll();

        Plugin GetById(string id);
    }
}
