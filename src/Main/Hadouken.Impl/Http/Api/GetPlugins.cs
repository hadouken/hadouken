using System.Linq;
using Hadouken.Http;
using Hadouken.Plugins;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("getplugins")]
    public class GetPlugins : ApiAction
    {
        private readonly IPluginEngine _pluginEngine;

        public GetPlugins(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public override ActionResult Execute()
        {
            return Json(new
                            {
                                plugins = (from m in _pluginEngine.Managers.Values
                                           select new
                                                      {
                                                          name = m.Name.ToLowerInvariant(),
                                                          version = m.Version.ToString(),
                                                          init = "boot.js" // should not hardcode this
                                                      }).ToArray()
                            });
        }
    }
}
