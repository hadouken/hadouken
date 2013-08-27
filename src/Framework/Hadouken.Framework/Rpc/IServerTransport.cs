using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface IServerTransport
    {
        void SetRequestCallback(Action<IRequest> callback);

        void Open();

        void Close();
    }
}
