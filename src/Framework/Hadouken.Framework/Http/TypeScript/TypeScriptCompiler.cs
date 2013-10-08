using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

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

            var process  = new Process
            {
                StartInfo =
                {
                    Arguments = String.Format("\"{0}\" --out \"{1}\"", file, Path.Combine(inputFileDirectory, fileName)),
                    WorkingDirectory = _toolsPath,
                    FileName = "tsc.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput =  true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            var output = new StringBuilder();
            var error = new StringBuilder();

            using (var outputWaitHandle = new AutoResetEvent(false))
            using (var errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        error.AppendLine(e.Data);
                    }
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (process.WaitForExit(10000) &&
                    outputWaitHandle.WaitOne(10000) &&
                    errorWaitHandle.WaitOne(10000))
                {
                    // Process completed. Check process.ExitCode here.
                    if (process.ExitCode != 0)
                    {
                        File.WriteAllText(Path.Combine(inputFileDirectory, fileName), error.ToString());
                    }
                }
            }

            return Path.Combine(inputFileDirectory, fileName);
        }

        public static ITypeScriptCompiler Create()
        {
            const string resourceBase = "Hadouken.Framework.Http.TypeScript.Resources.";
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
