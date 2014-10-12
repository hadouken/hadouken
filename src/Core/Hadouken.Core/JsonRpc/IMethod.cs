namespace Hadouken.Core.JsonRpc
{
    public interface IMethod
    {
        IParameter[] Parameters { get; }

        object Execute(object[] parameters);
    }
}
