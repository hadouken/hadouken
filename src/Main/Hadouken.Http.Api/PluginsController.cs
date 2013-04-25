using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Hadouken.Plugins;

namespace Hadouken.Http.Api
{
    public class PluginsController : ApiController
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsController(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public IEnumerable<object> Get()
        {
            return (from plugin in _pluginEngine.Plugins
                    select new
                        {
                            plugin.Name,
                            plugin.Version,
                            plugin.State
                        });
        } 
    }
}
