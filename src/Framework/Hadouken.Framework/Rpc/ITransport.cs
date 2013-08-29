using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface ITransport
    {
        bool SupportsScheme(string scheme);

        void SetRequestCallback(Action<IRequest> callback);

        void Open();

        void Close();
    }
}
