namespace Hadouken.Fx.JsonRpc
{
    public interface IParameterResolver
    {
        object[] Resolve(object requestParameters, IParameter[] targetParameters);
    }
}
