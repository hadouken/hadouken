using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using Hadouken.Fx;
using Hadouken.Fx.IO;
using Newtonsoft.Json;

namespace Hadouken.Plugins.Isolation
{
    public class ProcessIsolatedEnvironment : IIsolatedEnvironment
    {
        private static readonly int DefaultTimeout = 5000;

        private readonly IDirectory _baseDirectory;
        private readonly string _environmentId;
        private readonly EventWaitHandle _handle;
        private readonly StringBuilder _errorOutputBuilder = new StringBuilder();
        private readonly StringBuilder _outputBuilder = new StringBuilder();
        private readonly Process _hostProcess;
        private bool _firstLoad = true;
        private bool _isRunning;

        public ProcessIsolatedEnvironment(IDirectory baseDirectory)
        {
            _baseDirectory = baseDirectory;
            _environmentId = Guid.NewGuid().ToString();
            _handle = new EventWaitHandle(false, EventResetMode.AutoReset, _environmentId);
            _hostProcess = CreateProcess(_environmentId);
        }

        public event EventHandler UnhandledError;

        public void Load(IDictionary<string, object> configuration)
        {
            // Write the configuration to a mem-mapped file
            using(WriteConf(_environmentId, configuration))
            {
                // Clear our output buffers
                _errorOutputBuilder.Clear();
                _outputBuilder.Clear();

                // Start the process
                _hostProcess.Start();

                if (_firstLoad)
                {
                    _hostProcess.BeginErrorReadLine();
                    _hostProcess.BeginOutputReadLine();

                    _firstLoad = false;
                }

                // Wait until we're up and running
                if (!_handle.WaitOne(DefaultTimeout))
                {
                    if (!_hostProcess.HasExited)
                    {
                        _hostProcess.Kill();
                    }

                    throw new PluginException("Could not load plugin: " + _errorOutputBuilder);
                }

                _isRunning = true;
            }
        }

        public void Unload()
        {
            _isRunning = false;

            _handle.Set();

            if (_handle.WaitOne(DefaultTimeout)) return;

            if(!_hostProcess.HasExited)
            {
                _hostProcess.Kill();
            }
        }

        public long GetMemoryUsage()
        {
            if (_hostProcess == null || _hostProcess.HasExited)
            {
                return -1;
            }

            return _hostProcess.PrivateMemorySize64;
        }

        private void HostProcessOnExited(object sender, EventArgs eventArgs)
        {
            if (!_isRunning) return;

            var handler = UnhandledError;

            if (handler != null && _isRunning)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private Process CreateProcess(string environmentId)
        {
            var hostId = Process.GetCurrentProcess().Id;

            var info = new ProcessStartInfo
            {
                Arguments = string.Format("{0} {1}", environmentId, hostId),
                FileName = typeof(PluginHostProcess.Program).Assembly.Location,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _baseDirectory.FullPath
            };

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = info,
            };

            process.Exited += HostProcessOnExited;
            process.ErrorDataReceived += (sender, args) => _errorOutputBuilder.Append(args.Data);
            process.OutputDataReceived += (sender, args) => _outputBuilder.Append(args.Data);
            
            return process;
        }

        private MemoryMappedFile WriteConf(string envId, IDictionary<string, object> configuration)
        {
            var json = JsonConvert.SerializeObject(configuration);
            var data = Encoding.UTF8.GetBytes(json);
            var mmf = MemoryMappedFile.CreateNew(envId + ".config", data.Length*2);
            
            using (var stream = mmf.CreateViewStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(json);
            }

            return mmf;
        }
    }
}
