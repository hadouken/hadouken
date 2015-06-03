using System;
using System.Reflection;

namespace Hadouken.Core.JsonRpc {
    public class Parameter : IParameter {
        private readonly ParameterInfo _parameterInfo;

        public Parameter(ParameterInfo parameterInfo) {
            this._parameterInfo = parameterInfo;
        }

        public string Name {
            get { return this._parameterInfo.Name; }
        }

        public Type ParameterType {
            get { return this._parameterInfo.ParameterType; }
        }
    }
}