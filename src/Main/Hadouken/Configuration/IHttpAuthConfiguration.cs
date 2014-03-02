namespace Hadouken.Configuration
{
    public interface IHttpAuthConfiguration
    {
        string UserName { get; set; }

        string Password { get; set; }
    }
}
