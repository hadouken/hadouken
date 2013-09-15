using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JsonRpcMethodAttribute : Attribute
    {
        private readonly string _methodName;

        public JsonRpcMethodAttribute(string methodName)
        {
            _methodName = methodName;
        }

        public string MethodName
        {
            get { return _methodName; }
        }
    }
}
