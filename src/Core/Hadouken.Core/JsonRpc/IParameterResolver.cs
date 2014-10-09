namespace Hadouken.Core.JsonRpc
{
    public interface IParameterResolver
    {
        bool CanResolve(object requestParameters);

        object[] Resolve(object requestParameters, IParameter[] targetParameters);
    }
}
