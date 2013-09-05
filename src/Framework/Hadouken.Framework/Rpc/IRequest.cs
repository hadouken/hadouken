using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface IRequest
    {
        int? Id { get; }

        string Method { get; }

        string Protocol { get; }

        object GetParameterObject(Type type);
    }
}
