namespace Hadouken.Plugins.HttpJsonRpc
{
    public class AuthChangedEventArgs
    {
        public string UserName { get; set; }

        public string HashedPassword { get; set; }
    }
}