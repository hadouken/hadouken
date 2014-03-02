using System.ServiceModel;

namespace Hadouken.Fx.ServiceModel
{
    public class PluginServiceHost : IPluginServiceHost
    {
        private readonly ServiceHost _serviceHost;

        public PluginServiceHost(ServiceHost serviceHost)
        {
            _serviceHost = serviceHost;
        }

        public void Open()
        {
            _serviceHost.Open();
        }

        public void Close()
        {
            _serviceHost.Close();
        }
    }
}