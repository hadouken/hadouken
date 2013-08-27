using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RpcMethodAttribute : Attribute
    {
        private readonly string _name;

        public RpcMethodAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
