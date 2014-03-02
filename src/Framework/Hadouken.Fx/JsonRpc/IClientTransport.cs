namespace Hadouken.Fx.JsonRpc
{
    public interface IClientTransport
    {
        string Call(string json);
    }
}