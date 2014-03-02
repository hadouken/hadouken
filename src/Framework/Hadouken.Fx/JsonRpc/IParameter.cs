using System;

namespace Hadouken.Fx.JsonRpc
{
    public interface IParameter
    {
        string Name { get; }

        Type ParameterType { get; }
    }
}
