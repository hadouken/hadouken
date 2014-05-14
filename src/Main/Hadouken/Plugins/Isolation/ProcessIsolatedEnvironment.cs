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
        private readonly int _hostProcessId;
        private Process _hostProcess;
        private bool _isRunning;

        public ProcessIsolatedEnvironment(IDirectory baseDirectory)
        {
            _baseDirectory = baseDirectory;
            _environmentId = Guid.NewGuid().ToString();
            _handle = new EventWaitHandle(false, EventResetMode.AutoReset, _environmentId);
            _hostProcessId = Process.GetCurrentProcess().Id;
        }

        public event EventHandler UnhandledError;

        public void Load(IDictionary<string, object> configuration)
        {
            var startInfo = GetStartInfo();

            // Write the configuration to a mem-mapped file
            var mmf = WriteConf(_environmentId, configuration);      

            _hostProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            _hostProcess.Exited += HostProcessOnExited;
            _hostProcess.Start();

            // Wait until we're up and running
            if (!_handle.WaitOne(DefaultTimeout))
            {
                if (_hostProcess != null && !_hostProcess.HasExited)
                {
                    _hostProcess.Kill();
                }

                _hostProcess = null;

                throw new PluginException("Could not load plugin.");
            }

            _isRunning = true;

            mmf.Dispose();
        }

        public void Unload()
        {
            // Remove handler
            if (_hostProcess != null)
            {
                _hostProcess.Exited -= HostProcessOnExited;
            }

            _handle.Set();

            if (_hostProcess != null && !_hostProcess.HasExited)
            {
                _handle.WaitOne(DefaultTimeout);
                _hostProcess.WaitForExit();
            }

            _hostProcess = null;
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
            var handler = UnhandledError;

            if (handler != null && _isRunning)
            {
                handler(this, EventArgs.Empty);
            }

            // Remove the handler since the process is dead.
            _hostProcess.Exited -= HostProcessOnExited;
        }

        private ProcessStartInfo GetStartInfo()
        {
            return new ProcessStartInfo
            {
                Arguments = _environmentId + " " + _hostProcessId,
                FileName = typeof(PluginHostProcess.Program).Assembly.Location,
                UseShellExecute = false,
                WorkingDirectory = _baseDirectory.FullPath
            };
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
