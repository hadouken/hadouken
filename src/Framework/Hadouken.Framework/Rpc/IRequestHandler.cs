using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface IRequestHandler
    {
        JsonRpcResponse Execute(JsonRpcRequest request);
    }
}
