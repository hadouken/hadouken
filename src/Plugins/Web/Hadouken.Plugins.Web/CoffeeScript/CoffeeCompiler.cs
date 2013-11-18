using System;
using System.Threading;
using Jurassic;

namespace Hadouken.Plugins.Web.CoffeeScript
{
    public class CoffeeCompiler : ICoffeeCompiler
    {
        private const string CoffeeScriptCompile = "CoffeeScript.compile(Source, {{bare: {0}}})";
        private readonly ScriptEngine _scriptEngine;
        private readonly object _compileLock = new object();

        public CoffeeCompiler()
        {
            _scriptEngine = InitScriptEngine();
        }

        public string Compile(string source)
        {
            lock (_compileLock)
            {
                _scriptEngine.SetGlobalValue("Source", source);
                return _scriptEngine.Evaluate<string>(String.Format(CoffeeScriptCompile, "false"));
            }
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