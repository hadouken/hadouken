using System.Reflection;

namespace Hadouken.Fx.JsonRpc
{
    public interface IMethod
    {
        ParameterInfo[] Parameters { get; }

        object Execute(object[] parameters);
    }
}
