using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    public interface IRequest
    {
        object Id { get; }

        string Method { get; }

        string Protocol { get; }

        string GetParametersAsJson();
    }
}
