using System.ServiceModel;

namespace Hadouken.Framework.Wcf
{
    public class GenericServiceHost : IServiceHost
    {
        private readonly ServiceHost _serviceHost;

        public GenericServiceHost(ServiceHost serviceHost)
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
