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
                    Name = plugin.Name
                }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }
    }
}
