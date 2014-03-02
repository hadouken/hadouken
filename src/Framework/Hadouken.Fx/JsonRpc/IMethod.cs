namespace Hadouken.Fx.JsonRpc
{
    public interface IMethod
    {
        IParameter[] Parameters { get; }

        object Execute(object[] parameters);
    }
}
