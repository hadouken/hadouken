using System;
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
        private string _environmentId;
        private Process _hostProcess;
        private EventWaitHandle _runningHandle;

        public ProcessIsolatedEnvironment(IDirectory baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public event EventHandler UnhandledError;

        public void Load(PluginConfiguration configuration)
        {
            _environmentId = Guid.NewGuid().ToString();

            var loadEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _environmentId + ".load");
            var currentPid = Process.GetCurrentProcess().Id;

            var startInfo = new ProcessStartInfo
            {
                Arguments = _environmentId + " " + currentPid,
                FileName = typeof(PluginHostProcess.Program).Assembly.Location,
                UseShellExecute = false,
                WorkingDirectory = _baseDirectory.FullPath
            };

            // Write the configuration to a mem-mapped file
            var mmf = WriteConf(_environmentId, configuration);

            // The handle
            _runningHandle = new EventWaitHandle(false, EventResetMode.ManualReset, _environmentId + ".wait");            

            _hostProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            _hostProcess.Exited += HostProcessOnExited;
            _hostProcess.Start();

            // Wait until we're up and running
            if (!loadEvent.WaitOne(DefaultTimeout))
            {
                if (!_hostProcess.HasExited)
                {
                    _hostProcess.Kill();
                }

                _hostProcess = null;

                throw new PluginException("Could not load plugin.");
            }

            mmf.Dispose();
            loadEvent.Dispose();
        }

        public void Unload()
        {
            using (var unloadEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _environmentId + ".unload"))
            {
                // Remove handler
                _hostProcess.Exited -= HostProcessOnExited;

                _runningHandle.Set();

                if (!_hostProcess.HasExited)
                {
                    unloadEvent.WaitOne(DefaultTimeout);
                    _hostProcess.WaitForExit();
                }

                _hostProcess = null;
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
            var handler = UnhandledError;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            // Remove the handler since the process is dead.
            _hostProcess.Exited -= HostProcessOnExited;
        }

        private MemoryMappedFile WriteConf(string envId, PluginConfiguration configuration)
        {
            var json = JsonConvert.SerializeObject(configuration);
            var data = Encoding.UTF8.GetBytes(json);
            var mmf = MemoryMappedFile.CreateNew(envId, data.Length*2);
            
            using (var stream = mmf.CreateViewStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(json);
            }

            return mmf;
        }
    }
}
