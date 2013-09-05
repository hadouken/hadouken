using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface IClientTransport : IDisposable
    {
        Task<string> Send(string data);
    }
}
