namespace Hadouken.Fx.JsonRpc
{
    public interface IRequestHandler
    {
        string Handle(string request);
    }
}
