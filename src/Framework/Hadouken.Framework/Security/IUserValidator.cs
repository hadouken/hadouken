namespace Hadouken.Framework.Security
{
    public interface IUserValidator
    {
        bool IsValidUser(string userName, string password);
    }
}
