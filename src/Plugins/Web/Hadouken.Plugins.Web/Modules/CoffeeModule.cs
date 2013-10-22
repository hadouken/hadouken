using System.IO;
using Hadouken.Plugins.Web.CoffeeScript;
using Nancy;

namespace Hadouken.Plugins.Web.Modules
{
    public class CoffeeModule : NancyModule
    {
        private readonly IRootPathProvider _pathProvider;

        public CoffeeModule(IRootPathProvider pathProvider)
        {
            _pathProvider = pathProvider;

            Get[@"^(?<path>.*coffee)$"] = _ => Compile(_.path.ToString());
        }

        private dynamic Compile(string path)
        {
            return Response.AsFile("UI/" + path, "text/coffeescript");
        }
    }
}
