namespace Hadouken.Fx.JsonRpc
{
    public interface IMethodCache
    {
        IMethod Get(string methodName);
    }
}
