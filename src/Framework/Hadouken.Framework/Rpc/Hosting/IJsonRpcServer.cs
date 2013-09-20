using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Hosting
{
    public interface IJsonRpcServer
    {
        void Open();

        void Close();
    }
}
