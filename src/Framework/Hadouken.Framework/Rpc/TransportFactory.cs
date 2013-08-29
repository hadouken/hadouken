using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public class TransportFactory : ITransportFactory
    {
        private readonly IEnumerable<ITransport> _transports;

        public TransportFactory(IEnumerable<ITransport> transports)
        {
            _transports = transports;
        }

        public ITransport CreateTransport(string uri)
        {
            if (String.IsNullOrEmpty(uri))
                return null;

            var index = uri.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);

            if (index >= 0)
            {
                var scheme = uri.Substring(0, index);
                return _transports.FirstOrDefault(t => t.SupportsScheme(scheme));
            }

            return null;
        }
    }
}
