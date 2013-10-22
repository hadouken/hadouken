using System;
using System.IO;
using Jurassic;

namespace Hadouken.Plugins.Web.CoffeeScript
{
    public interface ICoffeeCompiler
    {
        string Compile(string source);
    }

    public class CoffeeCompiler : ICoffeeCompiler
    {
        private const string CoffeeScriptCompile = "CoffeeScript.compile(Source, {{bare: {0}}})";
        private readonly Lazy<ScriptEngine> _scriptEngine;

        public CoffeeCompiler()
        {
            _scriptEngine = new Lazy<ScriptEngine>(InitScriptEngine);
        }

        public string Compile(string source)
        {
            _scriptEngine.Value.SetGlobalValue("Source", source);
            return _scriptEngine.Value.Evaluate<string>(String.Format(CoffeeScriptCompile, "false"));
        }

        private ScriptEngine InitScriptEngine()
        {
            var engine = new ScriptEngine();
            var compilerScript =
                ResourceReader.Read("Hadouken.Plugins.Web.CoffeeScript.Resources.CoffeeScript.compiler.js");

            engine.Execute(compilerScript);

            return engine;
        }
    }

    public static class ResourceReader
    {
        public static string Read(string resourceName)
        {
            using (var resource = typeof (ResourceReader).Assembly.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                    return null;
                
                using (var reader = new StreamReader(resource))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
