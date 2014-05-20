namespace Hadouken.Security
{
    public interface IAuthenticationManager
    {
        bool IsValid(string userName, string password);
    }
}
