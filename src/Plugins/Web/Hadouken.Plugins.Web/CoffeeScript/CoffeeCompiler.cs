using System;
using System.Threading;
using Jurassic;

namespace Hadouken.Plugins.Web.CoffeeScript
{
    public class CoffeeCompiler : ICoffeeCompiler
    {
        private const string CoffeeScriptCompile = "CoffeeScript.compile(Source, {{bare: {0}}})";
        private readonly ThreadLocal<Lazy<ScriptEngine>> _scriptEngine;

        public CoffeeCompiler()
        {
            _scriptEngine = new ThreadLocal<Lazy<ScriptEngine>>(() => new Lazy<ScriptEngine>(InitScriptEngine));
        }

        public string Compile(string source)
        {
            _scriptEngine.Value.Value.SetGlobalValue("Source", source);
            return _scriptEngine.Value.Value.Evaluate<string>(String.Format(CoffeeScriptCompile, "false"));
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
}