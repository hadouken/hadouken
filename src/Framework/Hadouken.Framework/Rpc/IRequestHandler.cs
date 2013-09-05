namespace Hadouken.Framework.Rpc
{
    public interface IRequestHandler
    {
        IResponse Execute(IRequest request);
    }
}
