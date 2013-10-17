using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;

namespace Hadouken.Framework.Wcf
{
    public interface IProxyFactory<out T>
    {
        IProxy<T> Create(Uri endpoint);
    }
}
