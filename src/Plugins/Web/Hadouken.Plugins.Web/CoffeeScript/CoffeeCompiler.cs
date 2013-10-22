using System;
using Jurassic;

namespace Hadouken.Plugins.Web.CoffeeScript
{
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
}