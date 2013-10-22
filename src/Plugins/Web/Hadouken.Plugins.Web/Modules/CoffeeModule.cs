using System.IO;
using Hadouken.Plugins.Web.CoffeeScript;
using Nancy;

namespace Hadouken.Plugins.Web.Modules
{
    public class CoffeeModule : NancyModule
    {
        private readonly IRootPathProvider _pathProvider;
        private readonly ICoffeeCompiler _coffeeCompiler;

        public CoffeeModule(IRootPathProvider pathProvider, ICoffeeCompiler coffeeCompiler)
        {
            _pathProvider = pathProvider;
            _coffeeCompiler = coffeeCompiler;

            Get[@"^(?<path>.*js)$"] = _ => Compile(_.path.ToString());
        }

        private dynamic Compile(string path)
        {
            var coffeeScript = path.Substring(0, path.Length - 3) + ".coffee";
            coffeeScript = Path.Combine(_pathProvider.GetRootPath(), "UI", coffeeScript);

            if (!File.Exists(coffeeScript))
                return Response.AsFile("UI/" + path);

            var content = File.ReadAllText(coffeeScript);
            var compiledContent = _coffeeCompiler.Compile(content);

            return Response.AsText(compiledContent, "text/javascript");
        }
    }
}
