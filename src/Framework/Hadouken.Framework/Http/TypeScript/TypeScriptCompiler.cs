using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Hadouken.Framework.Http.TypeScript
{
    public class TypeScriptCompiler : ITypeScriptCompiler
    {
        private readonly string _toolsPath;

        protected TypeScriptCompiler(string toolsPath)
        {
            _toolsPath = toolsPath;
        }

        public string Compile(string file)
        {
            var inputFileDirectory = Path.GetDirectoryName(file);

            if (String.IsNullOrEmpty(inputFileDirectory))
                return null;

            var fileName = Path.GetFileNameWithoutExtension(file) + ".js";

            var p = new Process();
            p.StartInfo.Arguments = String.Format("{0} --out {1}", file, Path.Combine(inputFileDirectory, fileName));
            p.StartInfo.WorkingDirectory = _toolsPath;
            p.StartInfo.FileName = "tsc.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.Start();
            p.WaitForExit();

            return Path.Combine(inputFileDirectory, fileName);
        }

        public static ITypeScriptCompiler Create()
        {
            var resourceBase = "Hadouken.Framework.Http.TypeScript.Resources.";
            var temporaryPath = Path.Combine(Path.GetTempPath(), "tsc");

            if (!Directory.Exists(temporaryPath))
                Directory.CreateDirectory(temporaryPath);

            var assembly = typeof (TypeScriptCompiler).Assembly;

            // Extract resources to temporary folder
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith(resourceBase))
                    continue;

                var outputName = resourceName.Substring(resourceBase.Length);

                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                using (var outputStream = new StreamWriter(Path.Combine(temporaryPath, outputName)))
                {
                    if (resourceStream == null)
                        continue;

                    resourceStream.CopyTo(outputStream.BaseStream);
                }
            }

            return new TypeScriptCompiler(temporaryPath);
        }
    }
}
