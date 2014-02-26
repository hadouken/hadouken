using System;

namespace Hadouken.Plugins.Isolation
{
    public class AppDomainIsolatedEnvironment : IIsolatedEnvironment
    {
        private readonly string _baseDirectory;
        private Sandbox _sandbox;

        public AppDomainIsolatedEnvironment(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public void Load()
        {
            _sandbox = Sandbox.Create(_baseDirectory);
            _sandbox.Load();
        }

        public void Unload()
        {
            _sandbox.Unload();

            var domain = _sandbox.GetAppDomain();
            AppDomain.Unload(domain);
            _sandbox = null;
        }

        public long GetMemoryUsage()
        {
            if (_sandbox == null)
            {
                return -1;
            }

            return _sandbox.GetAppDomain().MonitoringSurvivedMemorySize;
        }
    }
}