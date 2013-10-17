using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Hadouken.Framework.TypeScript
{
    public interface ITypeScriptCompiler
    {
        /// <summary>
        /// Compiles the specified file and returns the compiled content.
        /// </summary>
        string Compile(string file);
    }

    public class TypeScriptCompiler : ITypeScriptCompiler
    {
        private bool _hasExtractedResources;
        private string _toolsPath;

        public string Compile(string file)
        {
            if (!_hasExtractedResources)
                ExtractResources();

            var outputFile = file.Substring(0, file.Length - 3) + ".js";

            var process = new Process
            {
                StartInfo =
                {
                    Arguments = String.Format("\"{0}\" --out \"{1}\"", file, outputFile),
                    WorkingDirectory = _toolsPath,
                    FileName = "tsc.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
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
                        File.WriteAllText(outputFile, error.ToString());
                    }
                }
            }

            return File.ReadAllText(outputFile);
        }

        private void ExtractResources()
        {
            var resourceBase = typeof (TypeScriptCompiler) + ".Resources.";
            var workingDirectory = Path.Combine(Path.GetTempPath(), "tsc", AppDomain.CurrentDomain.FriendlyName);
            var dir = new DirectoryInfo(workingDirectory);

            if (!dir.Exists)
                dir.Create();

            var assembly = typeof (TypeScriptCompiler).Assembly;

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith(resourceBase))
                    continue;

                var outputName = resourceName.Substring(resourceBase.Length);
                var outputPath = Path.Combine(workingDirectory, outputName);

                using (var writer = File.OpenWrite(outputPath))
                using(var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                        continue;

                    resourceStream.CopyTo(writer);
                }
            }

            _toolsPath = workingDirectory;
            _hasExtractedResources = true;
        }
    }
}
