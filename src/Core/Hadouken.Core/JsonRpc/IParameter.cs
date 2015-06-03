using System;

namespace Hadouken.Core.JsonRpc {
    public interface IParameter {
        string Name { get; }
        Type ParameterType { get; }
    }
}