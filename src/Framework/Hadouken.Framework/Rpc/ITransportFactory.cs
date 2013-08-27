using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface ITransportFactory
    {
        IServerTransport CreateServerTransport(Uri uri);

        IClientTransport CreateClientTransport(Uri uri);
    }
}
