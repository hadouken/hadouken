using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
