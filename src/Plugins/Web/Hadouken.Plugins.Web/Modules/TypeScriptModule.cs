using System.IO;
using Hadouken.Framework.TypeScript;
using Nancy;

namespace Hadouken.Plugins.WebUI.Modules
{
    public class TypeScriptModule : NancyModule
    {
        private readonly IRootPathProvider _pathProvider;
        private readonly ITypeScriptCompiler _compiler;

        public TypeScriptModule(IRootPathProvider pathProvider, ITypeScriptCompiler compiler)
        {
            _pathProvider = pathProvider;
            _compiler = compiler;

            Get[@"^(?<path>.*js)$"] = _ => Compile(_.path.ToString());
        }

        private dynamic Compile(string path)
        {
            var typeScript = path.Substring(0, path.Length - 3) + ".ts";
            typeScript = Path.Combine(_pathProvider.GetRootPath(), "UI", typeScript);

            if (!File.Exists(typeScript))
            {
                return Response.AsFile("UI/" + path);
            }

            var compiledContent = _compiler.Compile(typeScript);
            return Response.AsText(compiledContent, "text/javascript");
        }
    }
}
