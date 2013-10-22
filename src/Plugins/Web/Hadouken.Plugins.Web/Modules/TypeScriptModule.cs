using System.IO;
using Nancy;

namespace Hadouken.Plugins.Web.Modules
{
    public class TypeScriptModule : NancyModule
    {
        private readonly IRootPathProvider _pathProvider;

        public TypeScriptModule(IRootPathProvider pathProvider)
        {
            _pathProvider = pathProvider;

            Get[@"^(?<path>.*js)$"] = _ => Compile(_.path.ToString());
        }

        private dynamic Compile(string path)
        {
            return null;
        }
    }
}
