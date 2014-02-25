using System;
using System.Reflection;

namespace Hadouken.Fx.JsonRpc
{
    public class Parameter : IParameter
    {
        private readonly ParameterInfo _parameterInfo;

        public Parameter(ParameterInfo parameterInfo)
        {
            _parameterInfo = parameterInfo;
        }

        public string Name
        {
            get { return _parameterInfo.Name; }
        }

        public Type ParameterType
        {
            get { return _parameterInfo.ParameterType; }
        }
    }
}