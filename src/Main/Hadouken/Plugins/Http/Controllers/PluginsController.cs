using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Hadouken.Plugins.Http.Models;

namespace Hadouken.Plugins.Http.Controllers
{
    public class PluginsController : ApiController
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsController(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public HttpResponseMessage Get()
        {
            var dtos = (from plugin in _pluginEngine.GetAll()
                select new PluginListDto
                {
                    Name = plugin.Name,
                    Version = plugin.Version
                }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        public HttpResponseMessage Put(string id, PutPluginDto dto)
        {
            if (String.IsNullOrEmpty(id) || dto == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var plugin = _pluginEngine.Get(id);

            if (plugin == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            switch (dto.Action)
            {
                case PluginAction.Load:
                    if (plugin.State == PluginState.Loaded || plugin.State == PluginState.Loading)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    plugin.Load();
                    break;

                case PluginAction.Unload:
                    if (plugin.State == PluginState.Unloaded || plugin.State == PluginState.Unloading)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    plugin.Unload();
                    break;
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}
