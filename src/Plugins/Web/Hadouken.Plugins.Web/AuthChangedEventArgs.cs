namespace Hadouken.Plugins.Web
{
    public class AuthChangedEventArgs
    {
        public string UserName { get; set; }

        public string HashedPassword { get; set; }
    }
}